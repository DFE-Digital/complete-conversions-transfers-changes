using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Domain.Validators;
using Microsoft.Extensions.Logging;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Constants;
using Dfe.Complete.Application.Projects.Services;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverTransferProjectCommand(
    [Required]
    [Urn]
    int? Urn,
    [Required]
    [Ukprn]
    int? IncomingTrustUkprn,
    [Required]
    [Ukprn]
    int? OutgoingTrustUkprn,
    [Required]
    [PastDate (AllowToday = true)]
    DateOnly? AdvisoryBoardDate,
    [Required]
    [FirstOfMonthDate]
    DateOnly? ProvisionalTransferDate,
    [Required]
    [InternalEmail]
    string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required] bool? InadequateOfsted,
    [Required] bool? FinancialSafeguardingGovernanceIssues,
    [Required] bool? OutgoingTrustToClose,
    string? AdvisoryBoardConditions,
    [GroupReferenceNumber]
    string? GroupId = null) : IRequest<ProjectId>;

public class CreateHandoverTransferProjectCommandHandler(
    IUnitOfWork unitOfWork,
    ITrustsV4Client trustClient,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverTransferProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverTransferProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            // Validate the request
            await ValidateRequest(request, cancellationToken);

            var projectId = new ProjectId(Guid.NewGuid());
            var urn = request.Urn!.Value;
            var localAuthorityId = await handoverProjectService.GetLocalAuthorityForUrn(urn, cancellationToken);
            var region = await handoverProjectService.GetRegionForUrn(urn, cancellationToken);
            var group = await handoverProjectService.GetGroupForGroupId(request.GroupId, cancellationToken);

            if (group != null) handoverProjectService.ValidateGroupId(group, request.IncomingTrustUkprn!.Value);

            ProjectGroupId? groupId = null;
            if (group != null) groupId = group.Id;
            if (group == null && !string.IsNullOrWhiteSpace(request.GroupId)) groupId = await handoverProjectService.CreateProjectGroup(request.GroupId, request.IncomingTrustUkprn!.Value, cancellationToken);

            var userDto = new UserDto
            {
                FirstName = request.CreatedByFirstName,
                LastName = request.CreatedByLastName,
                Email = request.CreatedByEmail,
                Team = region.ToDescription()
            };
            var userId = await handoverProjectService.GetOrCreateUserAsync(userDto, cancellationToken);

            // Create transfer task data
            var transferTask = handoverProjectService.CreateTransferTaskAsync(
                request.InadequateOfsted!.Value,
                request.FinancialSafeguardingGovernanceIssues!.Value,
                request.OutgoingTrustToClose!.Value
            );

            var parameters = new CreateHandoverTransferProjectParams(
                projectId,
                new Urn(urn),
                transferTask.Id.Value,
                request.ProvisionalTransferDate!.Value,
                new Ukprn(request.IncomingTrustUkprn!.Value),
                new Ukprn(request.OutgoingTrustUkprn!.Value),
                region,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                groupId,
                userId,
                localAuthorityId);

            var project = Project.CreateHandoverTransferProject(parameters);

            project.PrepareId = request.PrepareId!.Value;

            await handoverProjectService.SaveProjectAndTaskAsync(project, transferTask, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating handover transfer project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover transfer project for URN: {request.Urn}", ex);
        }
    }

    private async Task ValidateRequest(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        if (request.IncomingTrustUkprn == request.OutgoingTrustUkprn)
            throw new ValidationException(ValidationConstants.SameTrustValidationMessage);

        // Check if URN already exists in active/inactive transfer projects
        var existingProject = await handoverProjectService.FindExistingProjectAsync(request.Urn!.Value, cancellationToken);

        if (existingProject != null)
            throw new ValidationException(string.Format(ValidationConstants.UrnExistsValidationMessage, request.Urn));

        _ = await trustClient.GetTrustByUkprn2Async(request.IncomingTrustUkprn!.Value.ToString(), cancellationToken) ?? throw new ValidationException(string.Format(ValidationConstants.NoTrustFoundValidationMessage, request.IncomingTrustUkprn.Value));
        _ = await trustClient.GetTrustByUkprn2Async(request.OutgoingTrustUkprn!.Value.ToString(), cancellationToken) ?? throw new ValidationException(string.Format(ValidationConstants.NoTrustFoundValidationMessage, request.OutgoingTrustUkprn.Value));
    }
}
