using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.TaskData
{
    public record GetTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<TaskDataModel?>>;
    public class GetTaskDataByProjectIdQueryHandler(IProjectReadRepository projectReadRepository,
        ITaskDataReadRepository taskDataReadRepository, ILogger<GetTaskDataByProjectIdQueryHandler> logger) : IRequestHandler<GetTaskDataByProjectIdQuery, Result<TaskDataModel?>>
    {
        public async Task<Result<TaskDataModel?>> Handle(GetTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectReadRepository.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken) ??
                    throw new NotFoundException($"Project with ID {request.ProjectId} not found.");

                if (project.Type == ProjectType.Conversion)
                {
                    var taskData = await GetConversionTaskDataAsync(project.TasksDataId, cancellationToken);
                    return Result<TaskDataModel?>.Success(taskData);
                }
                else
                {
                    var taskData = await GetTransferTaskDataAsync(project.TasksDataId, cancellationToken);
                    return Result<TaskDataModel?>.Success(taskData);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving task data for project {ProjectId}", request.ProjectId);
                return Result<TaskDataModel?>.Failure(ex.Message);
            }
        }
        private async Task<TaskDataModel> GetConversionTaskDataAsync(TaskDataId? taskDataId, CancellationToken cancellationToken)
        {
            var taskData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(t => t.Id == taskDataId, cancellationToken) ??
                throw new NotFoundException($"Conversion task data with ID {taskDataId} not found.");

            return TaskDataModel.MapConversionTaskDataToModel(taskData);
        }
        private async Task<TaskDataModel> GetTransferTaskDataAsync(TaskDataId? taskDataId, CancellationToken cancellationToken)
        {
            var taskData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(t => t.Id == taskDataId, cancellationToken) ??
                throw new NotFoundException($"Transfer task data with ID {taskDataId} not found.");

            return TaskDataModel.MapTransferTaskDataToModel(taskData);
        }
    }
}
