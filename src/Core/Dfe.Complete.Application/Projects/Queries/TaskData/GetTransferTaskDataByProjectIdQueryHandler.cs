using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.TaskData
{
    public record GetTransferTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<TransferTaskDataModel?>>;
    public class GetTransferTaskDataByProjectIdQueryHandler(IProjectReadRepository projectReadRepository,
        ITaskDataReadRepository taskDataReadRepository, ILogger<GetTransferTaskDataByProjectIdQueryHandler> logger) : IRequestHandler<GetTransferTaskDataByProjectIdQuery, Result<TransferTaskDataModel?>>
    {
        public async Task<Result<TransferTaskDataModel?>> Handle(GetTransferTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectReadRepository.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken) ??
                    throw new NotFoundException($"Project with ID {request.ProjectId} not found.");

                var taskData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(t => t.Id == project.TasksDataId, cancellationToken) 
                    ?? throw new NotFoundException($"Transfer task data with ID {project.TasksDataId} not found.");

                return Result<TransferTaskDataModel?>.Success(TransferTaskDataModel.MapTransferTaskDataToModel(taskData)); 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving task data for project {ProjectId}", request.ProjectId);
                return Result<TransferTaskDataModel?>.Failure(ex.Message);
            }
        }
    }
}