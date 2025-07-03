using Dfe.Complete.Application.Projects.Common;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Projects.Queries.ConversionTasks;

public record ConversionHandoverTaskDataDto(bool? NotApplicable, bool? ReviewProjectInformation, bool? MakeNotes, bool? AttendHandoverMeeting);


public record GetHandoverTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<ConversionHandoverTaskDataDto>>;

public class GetHandoverTasksDataByProjectIdQueryHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository)
    : IRequestHandler<GetHandoverTaskDataByProjectIdQuery, Result<ConversionHandoverTaskDataDto>>
{
    public async Task<Result<ConversionHandoverTaskDataDto>> Handle(GetHandoverTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
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
            var dto = new ConversionHandoverTaskDataDto(conversionTasks.HandoverNotApplicable, conversionTasks.HandoverReview, conversionTasks.HandoverNotes, conversionTasks.HandoverMeeting);
            return Result<ConversionHandoverTaskDataDto>.Success(dto);
        }

        return Result<ConversionHandoverTaskDataDto>.Failure("Unsupported project type for handover tasks.");

    }
}
