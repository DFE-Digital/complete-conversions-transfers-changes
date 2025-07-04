using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models.ConversionTasks;

namespace Dfe.Complete.Application.Projects.Queries.ConversionTasks;

public record GetConversionHandoverTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<ConversionHandoverTaskDataDto>>;

public class GetConversionHandoverTasksDataByProjectIdQueryHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository)
    : IRequestHandler<GetConversionHandoverTaskDataByProjectIdQuery, Result<ConversionHandoverTaskDataDto>>
{
    public async Task<Result<ConversionHandoverTaskDataDto>> Handle(GetConversionHandoverTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<ConversionHandoverTaskDataDto>.Failure("Project not found.");
        }

        if (project.TasksDataId == null)
        {
            var dto = new ConversionHandoverTaskDataDto(null, null, null, null);
            return Result<ConversionHandoverTaskDataDto>.Success(dto);
        }

        if (project.Type == ProjectType.Conversion)
        {
            var conversionTasks = await conversionTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
            var dto = new ConversionHandoverTaskDataDto(
                conversionTasks.HandoverNotApplicable,
                conversionTasks.HandoverReview,
                conversionTasks.HandoverNotes,
                conversionTasks.HandoverMeeting);
            return Result<ConversionHandoverTaskDataDto>.Success(dto);
        }

        return Result<ConversionHandoverTaskDataDto>.Failure("Unsupported project type for handover tasks.");

    }
}
