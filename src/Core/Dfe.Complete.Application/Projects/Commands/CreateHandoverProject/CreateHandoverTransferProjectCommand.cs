using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverTransferProjectCommand(
    [Required][Urn] int? Urn,
    [Required][Ukprn(ValueIsInteger = true)] int? IncomingTrustUkprn,
    [Required][Ukprn(ValueIsInteger = true)] int? OutgoingTrustUkprn,
    [Required][PastDate(AllowToday = true)] DateOnly? AdvisoryBoardDate,
    [Required][FirstOfMonthDate] DateOnly? ProvisionalTransferDate,
    [Required][InternalEmail] string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required] bool? InadequateOfsted,
    [Required] bool? FinancialSafeguardingGovernanceIssues,
    [Required] bool? OutgoingTrustToClose,
    string? AdvisoryBoardConditions,
    [GroupReferenceNumber] string? GroupId = null) : IRequest<ProjectId>;

public class CreateHandoverTransferProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverTransferProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverTransferProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;
            var incomingTrustUkprn = request.IncomingTrustUkprn!.Value;
            var outgoingTrustUkprn = request.OutgoingTrustUkprn!.Value;

            // Validate the request
            if (incomingTrustUkprn == outgoingTrustUkprn)
                throw new ValidationException(Constants.ValidationConstants.SameTrustValidationMessage);

            await handoverProjectService.ValidateUrnAsync(urn, cancellationToken: cancellationToken);
            await handoverProjectService.ValidateTrustAsync(incomingTrustUkprn, cancellationToken: cancellationToken);
            await handoverProjectService.ValidateTrustAsync(outgoingTrustUkprn, cancellationToken: cancellationToken);

            // Prepare common project data
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.CreatedByFirstName,
                request.CreatedByLastName,
                request.CreatedByEmail,
                cancellationToken);

            ProjectGroupId? groupId = null;
            if (!string.IsNullOrWhiteSpace(request.GroupId))
                groupId = await handoverProjectService.GetOrCreateProjectGroup(request.GroupId!, incomingTrustUkprn, cancellationToken);

            // Create transfer task data
            var transferTask = handoverProjectService.CreateTransferTaskAsync(
                request.InadequateOfsted!.Value,
                request.FinancialSafeguardingGovernanceIssues!.Value,
                request.OutgoingTrustToClose!.Value
            );

            var parameters = new CreateHandoverTransferProjectParams(
                commonData.ProjectId,
                new Urn(commonData.Urn),
                transferTask.Id.Value,
                request.ProvisionalTransferDate!.Value,
                new Ukprn(incomingTrustUkprn),
                new Ukprn(outgoingTrustUkprn),
                commonData.Region,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                groupId,
                commonData.UserId,
                commonData.LocalAuthorityId);

            var project = Project.CreateHandoverTransferProject(parameters);

            project.PrepareId = request.PrepareId!.Value;

            await handoverProjectService.SaveProjectAndTaskAsync(project, transferTask, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (AcademiesApiException ex)
        {
            if (ex.StatusCode == 404)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, "Exception while creating handover transfer project for URN: {Urn}", request.Urn);
                throw new UnprocessableContentException(ex.Message, ex);
            }
            throw new UnknownException(ex.Message);
        }
        catch (Exception ex) when (ex is not UnprocessableContentException && ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating handover transfer project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover transfer project for URN: {request.Urn}", ex);
        }
    }
}
