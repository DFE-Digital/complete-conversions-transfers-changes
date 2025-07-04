using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.TransferTasks;

public record UpdateTransferFormMTaskDataByProjectIdCommand
(
    ProjectId ProjectId,
    bool? NotApplicable = null,
    bool? ReceivedFormM = null,
    bool? ReceivedTitlePlans = null,
    bool? Cleared = null,
    bool? Signed = null,
    bool? Saved = null
) : IRequest<Result<Unit>>;

public class UpdateTransferFormMTaskDataByProjectIdCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository) : IRequestHandler<UpdateTransferFormMTaskDataByProjectIdCommand, Result<Unit>>
{
    private readonly ICompleteRepository<Project> _projectRepository = projectRepository;
    private readonly ICompleteRepository<TransferTasksData> _transferTaskRepository = transferTaskRepository;

    public async Task<Result<Unit>> Handle(UpdateTransferFormMTaskDataByProjectIdCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<Unit>.Failure("Project not found.");
        }

        if (project.Type != ProjectType.Transfer)
        {
            return Result<Unit>.Failure("Unsupported project type for Form M tasks.");
        }

        if (project.TasksDataId == null)
        {
            return Result<Unit>.Failure("Project does not have associated transfer tasks data.");
        }

        var transferTasks = await _transferTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
        if (transferTasks == null)
        {
            return Result<Unit>.Failure("Transfer tasks data not found.");
        }

        transferTasks.FormMNotApplicable = request.NotApplicable;
        transferTasks.FormMReceivedFormM = request.ReceivedFormM;
        transferTasks.FormMReceivedTitlePlans = request.ReceivedTitlePlans;
        transferTasks.FormMCleared = request.Cleared;
        transferTasks.FormMSigned = request.Signed;
        transferTasks.FormMSaved = request.Saved;

        await _transferTaskRepository.UpdateAsync(transferTasks, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
