using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
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
    [GroupReferenceNumber] string? GroupId = null) : IRequest<ProjectId>, IBaseHandoverTransferProjectCommand;

public class CreateHandoverTransferProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverTransferProjectCommandHandler> logger)
    : BaseCreateHandoverTransferProjectCommandHandler<CreateHandoverTransferProjectCommand>(unitOfWork, handoverProjectService, logger),
      IRequestHandler<CreateHandoverTransferProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }

    protected override async Task PerformSpecificValidationsAsync(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        var incomingTrustUkprn = request.IncomingTrustUkprn!.Value;
        var outgoingTrustUkprn = request.OutgoingTrustUkprn!.Value;

        // Validate that incoming and outgoing trusts are different
        if (incomingTrustUkprn == outgoingTrustUkprn)
            throw new ValidationException(Constants.ValidationConstants.SameTrustValidationMessage);

        // Validate incoming trust exists
        await _handoverProjectService.ValidateTrustAsync(incomingTrustUkprn, cancellationToken: cancellationToken);
    }

    protected override async Task<SpecificProjectData> GetSpecificProjectDataAsync(CreateHandoverTransferProjectCommand request, CancellationToken cancellationToken)
    {
        var incomingTrustUkprn = request.IncomingTrustUkprn!.Value;

        ProjectGroupId? groupId = null;
        if (!string.IsNullOrWhiteSpace(request.GroupId))
            groupId = await _handoverProjectService.GetOrCreateProjectGroup(request.GroupId!, incomingTrustUkprn, cancellationToken);

        return new SpecificProjectData
        {
            GroupId = groupId,
            IncomingTrustUkprn = new Ukprn(incomingTrustUkprn)
        };
    }

    protected override Project CreateProject(
        CreateHandoverTransferProjectCommand request,
        HandoverProjectCommonData commonData,
        SpecificProjectData specificData,
        TransferTasksData transferTask,
        CancellationToken cancellationToken)
    {
        var parameters = new CreateHandoverTransferProjectParams(
            commonData.ProjectId,
            new Urn(commonData.Urn),
            transferTask.Id.Value,
            request.ProvisionalTransferDate!.Value,
            specificData.IncomingTrustUkprn!,
            new Ukprn(request.OutgoingTrustUkprn!.Value),
            commonData.Region,
            request.AdvisoryBoardDate!.Value,
            request.AdvisoryBoardConditions ?? null,
            specificData.GroupId,
            commonData.UserId,
            commonData.LocalAuthorityId);

        return Project.CreateHandoverTransferProject(parameters);
    }
}
