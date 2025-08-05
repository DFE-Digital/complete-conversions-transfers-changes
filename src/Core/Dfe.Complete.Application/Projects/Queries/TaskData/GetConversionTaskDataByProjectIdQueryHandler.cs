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
    public record GetConversionTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<ConversionTaskDataModel?>>;
    public class GetConversionTaskDataByProjectIdQueryHandler(IProjectReadRepository projectReadRepository,
        ITaskDataReadRepository taskDataReadRepository, ILogger<GetConversionTaskDataByProjectIdQueryHandler> logger) : IRequestHandler<GetConversionTaskDataByProjectIdQuery, Result<ConversionTaskDataModel?>>
    {
        public async Task<Result<ConversionTaskDataModel?>> Handle(GetConversionTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectReadRepository.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken) ??
                    throw new NotFoundException($"Project with ID {request.ProjectId} not found.");

                var taskData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(t => t.Id == project.TasksDataId, cancellationToken)
                    ?? throw new NotFoundException($"Conversion task data with ID {project.TasksDataId} not found.");

                return Result<ConversionTaskDataModel?>.Success(ConversionTaskDataModel.MapConversionTaskDataToModel(taskData));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving task data for project {ProjectId}", request.ProjectId);
                return Result<ConversionTaskDataModel?>.Failure(ex.Message);
            }
        }
    }
}