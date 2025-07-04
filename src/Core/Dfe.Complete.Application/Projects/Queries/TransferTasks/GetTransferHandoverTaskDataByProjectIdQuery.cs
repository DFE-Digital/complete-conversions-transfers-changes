using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Queries.TransferTasks;

public record GetTransferHandoverTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<TransferHandoverTaskDataDto>>;

public class GetTransferHandoverTasksDataByProjectIdQueryHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository)
    : IRequestHandler<GetTransferHandoverTaskDataByProjectIdQuery, Result<TransferHandoverTaskDataDto>>
{
    public async Task<Result<TransferHandoverTaskDataDto>> Handle(GetTransferHandoverTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<TransferHandoverTaskDataDto>.Failure("Project not found.");
        }

        if (project.TasksDataId == null)
        {
            var dto = new TransferHandoverTaskDataDto(null, null, null, null);
            return Result<TransferHandoverTaskDataDto>.Success(dto);
        }

        if (project.Type == ProjectType.Transfer)
        {
            var transferTasks = await transferTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
            var dto = new TransferHandoverTaskDataDto(
                transferTasks.HandoverNotApplicable,
                transferTasks.HandoverReview,
                transferTasks.HandoverNotes,
                transferTasks.HandoverMeeting);
            return Result<TransferHandoverTaskDataDto>.Success(dto);
        }

        return Result<TransferHandoverTaskDataDto>.Failure("Unsupported project type for handover tasks.");

    }
}
