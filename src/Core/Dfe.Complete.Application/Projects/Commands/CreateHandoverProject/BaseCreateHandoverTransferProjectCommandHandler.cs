using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public abstract class BaseCreateHandoverTransferProjectCommandHandler<TRequest>
    where TRequest : IBaseHandoverTransferProjectCommand
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IHandoverProjectService _handoverProjectService;
    protected readonly ILogger _logger;

    protected BaseCreateHandoverTransferProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IHandoverProjectService handoverProjectService,
        ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _handoverProjectService = handoverProjectService;
        _logger = logger;
    }

    protected async Task<ProjectId> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;
            var outgoingTrustUkprn = request.OutgoingTrustUkprn!.Value;

            // Validate URN and outgoing trust (common to both)
            await _handoverProjectService.ValidateUrnAsync(urn, cancellationToken);
            await PerformSpecificValidationsAsync(request, cancellationToken);
            await _handoverProjectService.ValidateTrustAsync(outgoingTrustUkprn, cancellationToken);

            // Perform command-specific validations

            // Prepare common project data
            var commonData = await _handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.CreatedByFirstName,
                request.CreatedByLastName,
                request.CreatedByEmail,
                cancellationToken);

            // Get command-specific data
            var specificData = await GetSpecificProjectDataAsync(request, cancellationToken);

            // Create transfer task data
            var transferTask = _handoverProjectService.CreateTransferTaskAsync(
                request.InadequateOfsted!.Value,
                request.FinancialSafeguardingGovernanceIssues!.Value,
                request.OutgoingTrustToClose!.Value
            );

            // Create project using command-specific parameters
            var project = CreateProject(request, commonData, specificData, transferTask, cancellationToken);

            project.PrepareId = request.PrepareId!.Value;

            await _handoverProjectService.SaveProjectAndTaskAsync(project, transferTask, cancellationToken);

            await _unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (AcademiesApiException ex)
        {
            if (ex.StatusCode == 404)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Exception while creating handover transfer project for URN: {Urn}", request.Urn);
                throw new UnprocessableContentException(ex.Message, ex);
            }
            throw new UnknownException(ex.Message);
        }
        catch (Exception ex) when (ex is not UnprocessableContentException && ex is not NotFoundException && ex is not ValidationException)
        {
            await _unitOfWork.RollBackAsync();
            _logger.LogError(ex, "Exception while creating handover transfer project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover transfer project for URN: {request.Urn}", ex);
        }
    }

    protected abstract Task PerformSpecificValidationsAsync(TRequest request, CancellationToken cancellationToken);
    protected abstract Task<SpecificProjectData> GetSpecificProjectDataAsync(TRequest request, CancellationToken cancellationToken);
    protected abstract Project CreateProject(
        TRequest request,
        HandoverProjectCommonData commonData,
        SpecificProjectData specificData,
        TransferTasksData transferTask,
        CancellationToken cancellationToken);
}

public interface IBaseHandoverTransferProjectCommand
{
    int? Urn { get; }
    DateOnly? AdvisoryBoardDate { get; }
    DateOnly? ProvisionalTransferDate { get; }
    string CreatedByEmail { get; }
    string CreatedByFirstName { get; }
    string CreatedByLastName { get; }
    int? PrepareId { get; }
    int? OutgoingTrustUkprn { get; }
    bool? InadequateOfsted { get; }
    bool? FinancialSafeguardingGovernanceIssues { get; }
    bool? OutgoingTrustToClose { get; }
    string? AdvisoryBoardConditions { get; }
}

public class SpecificProjectData
{
    public ProjectGroupId? GroupId { get; set; }
    public Ukprn? IncomingTrustUkprn { get; set; }
    public string? NewTrustReferenceNumber { get; set; }
    public string? NewTrustName { get; set; }
}