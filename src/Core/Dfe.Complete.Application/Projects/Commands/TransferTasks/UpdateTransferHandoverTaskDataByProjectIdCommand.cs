using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.TransferTasks;

public record UpdateTransferHandoverTaskDataByProjectIdCommand
(
    ProjectId ProjectId,
    bool? NotApplicable = null,
    bool? ReviewProjectInformation = null,
    bool? MakeNotes = null,
    bool? AttendHandoverMeeting = null
) : IRequest<Result<Unit>>;

public class UpdateTransferHandoverTaskDataByProjectIdCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository) : IRequestHandler<UpdateTransferHandoverTaskDataByProjectIdCommand, Result<Unit>>
{
    private readonly ICompleteRepository<Project> _projectRepository = projectRepository;
    private readonly ICompleteRepository<TransferTasksData> _transferTaskRepository = transferTaskRepository;

    public async Task<Result<Unit>> Handle(UpdateTransferHandoverTaskDataByProjectIdCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<Unit>.Failure("Project not found.");
        }

        if (project.Type != ProjectType.Transfer)
        {
            return Result<Unit>.Failure("Unsupported project type for handover tasks.");
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

        transferTasks.HandoverNotApplicable = request.NotApplicable;
        transferTasks.HandoverReview = request.ReviewProjectInformation;
        transferTasks.HandoverNotes = request.MakeNotes;
        transferTasks.HandoverMeeting = request.AttendHandoverMeeting;

        await _transferTaskRepository.UpdateAsync(transferTasks, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
