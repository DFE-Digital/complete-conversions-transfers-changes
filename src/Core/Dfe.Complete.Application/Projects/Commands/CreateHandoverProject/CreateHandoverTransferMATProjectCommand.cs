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

public record CreateHandoverTransferMatProjectCommand(
    [Required][Urn] int? Urn,
    [Required][Trn] string? NewTrustReferenceNumber,
    [Required] string? NewTrustName,
    [Required][PastDate(AllowToday = true)] DateOnly? AdvisoryBoardDate,
    [Required][FirstOfMonthDate] DateOnly? ProvisionalTransferDate,
    [Required][InternalEmail] string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required][Ukprn(ValueIsInteger = true)] int? OutgoingTrustUkprn,
    [Required] bool? InadequateOfsted,
    [Required] bool? FinancialSafeguardingGovernanceIssues,
    [Required] bool? OutgoingTrustToClose,
    string? AdvisoryBoardConditions) : IRequest<ProjectId>;

public class CreateHandoverTransferMatProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverTransferMatProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverTransferMatProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverTransferMatProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;
            var outgoingTrustUkprn = request.OutgoingTrustUkprn!.Value;

            // Validate the request
            await handoverProjectService.ValidateUrnAsync(urn, cancellationToken);
            await handoverProjectService.ValidateTrustAsync(outgoingTrustUkprn, cancellationToken: cancellationToken);

            // Prepare common project data
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.CreatedByFirstName,
                request.CreatedByLastName,
                request.CreatedByEmail,
                cancellationToken);


            // Create transfer task data
            var transferTask = handoverProjectService.CreateTransferTaskAsync(
                request.InadequateOfsted!.Value,
                request.FinancialSafeguardingGovernanceIssues!.Value,
                request.OutgoingTrustToClose!.Value
            );

            var parameters = new CreateHandoverTransferMatProjectParams(
                commonData.ProjectId,
                new Urn(commonData.Urn),
                transferTask.Id.Value,
                request.ProvisionalTransferDate!.Value,
                new Ukprn(request.OutgoingTrustUkprn!.Value),
                commonData.Region,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions,
                commonData.UserId,
                commonData.LocalAuthorityId,
                request.NewTrustReferenceNumber!,
                request.NewTrustName!
                );

            var project = Project.CreateHandoverTransferMATProject(parameters);

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
