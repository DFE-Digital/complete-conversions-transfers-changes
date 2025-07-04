using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.ConversionTasks;

public record UpdateConversionHandoverTaskDataByProjectIdCommand
(
    ProjectId ProjectId,
    bool? NotApplicable = null,
    bool? ReviewProjectInformation = null,
    bool? MakeNotes = null,
    bool? AttendHandoverMeeting = null
) : IRequest<Result<Unit>>;

public class UpdateConversionHandoverTaskDataByProjectIdCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository) : IRequestHandler<UpdateConversionHandoverTaskDataByProjectIdCommand, Result<Unit>>
{
    private readonly ICompleteRepository<Project> _projectRepository = projectRepository;
    private readonly ICompleteRepository<ConversionTasksData> _conversionTaskRepository = conversionTaskRepository;

    public async Task<Result<Unit>> Handle(UpdateConversionHandoverTaskDataByProjectIdCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result<Unit>.Failure("Project not found.");
        }

        if (project.Type != ProjectType.Conversion)
        {
            return Result<Unit>.Failure("Unsupported project type for handover tasks.");
        }

        if (project.TasksDataId == null)
        {
            return Result<Unit>.Failure("Project does not have associated conversion tasks data.");
        }

        var conversionTasks = await _conversionTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
        if (conversionTasks == null)
        {
            return Result<Unit>.Failure("Conversion tasks data not found.");
        }

        conversionTasks.HandoverNotApplicable = request.NotApplicable;
        conversionTasks.HandoverReview = request.ReviewProjectInformation;
        conversionTasks.HandoverNotes = request.MakeNotes;
        conversionTasks.HandoverMeeting = request.AttendHandoverMeeting;

        await _conversionTaskRepository.UpdateAsync(conversionTasks, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
