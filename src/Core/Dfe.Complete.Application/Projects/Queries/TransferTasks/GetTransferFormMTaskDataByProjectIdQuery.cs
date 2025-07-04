using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Queries.TransferTasks;

public record GetTransferFormMTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<TransferFormMTaskDataDto>>;

public class GetTransferFormMTaskDataByProjectIdQueryHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository)
    : IRequestHandler<GetTransferFormMTaskDataByProjectIdQuery, Result<TransferFormMTaskDataDto>>
{
    public async Task<Result<TransferFormMTaskDataDto>> Handle(GetTransferFormMTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<TransferFormMTaskDataDto>.Failure("Project not found.");
        }

        if (project.TasksDataId == null)
        {
            var dto = new TransferFormMTaskDataDto(null, null, null, null, null, null);
            return Result<TransferFormMTaskDataDto>.Success(dto);
        }

        if (project.Type == ProjectType.Transfer)
        {
            var transferTasks = await transferTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
            var dto = new TransferFormMTaskDataDto(
                transferTasks.FormMNotApplicable,
                transferTasks.FormMReceivedFormM,
                transferTasks.FormMReceivedTitlePlans,
                transferTasks.FormMCleared,
                transferTasks.FormMSigned,
                transferTasks.FormMSaved);
            return Result<TransferFormMTaskDataDto>.Success(dto);
        }

        return Result<TransferFormMTaskDataDto>.Failure("Unsupported project type for Form M tasks.");

    }
}
