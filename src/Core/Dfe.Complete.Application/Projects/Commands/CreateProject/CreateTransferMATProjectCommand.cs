using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateTransferMatProjectCommand(
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
    string? AdvisoryBoardConditions,
    [GroupReferenceNumber] string? GroupId = null) : IRequest<ProjectId>, IBaseHandoverTransferProjectCommand;

public class CreateTransferMatProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateTransferMatProjectCommandHandler> logger)
    : BaseCreateTransferProjectCommandHandler<CreateTransferMatProjectCommand>(unitOfWork, handoverProjectService, logger),
      IRequestHandler<CreateTransferMatProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateTransferMatProjectCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }

    protected override Task PerformSpecificValidationsAsync(CreateTransferMatProjectCommand request, CancellationToken cancellationToken)
    {
        return _handoverProjectService.ValidateTrnAndTrustNameAsync(
            request.NewTrustReferenceNumber!,
            request.NewTrustName!,
            cancellationToken);
    }

    protected override Task<SpecificProjectData> GetSpecificProjectDataAsync(CreateTransferMatProjectCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SpecificProjectData
        {
            NewTrustReferenceNumber = request.NewTrustReferenceNumber!,
            NewTrustName = request.NewTrustName!
        });
    }

    protected override Project CreateProject(
        CreateTransferMatProjectCommand request,
        HandoverProjectCommonData commonData,
        SpecificProjectData specificData,
        TransferTasksData transferTask,
        CancellationToken cancellationToken)
    {
        var parameters = new CreateTransferMatProjectParams(
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
            specificData.NewTrustReferenceNumber!,
            specificData.NewTrustName!
        );

        return Project.CreateTransferMATProject(parameters);
    }
}
