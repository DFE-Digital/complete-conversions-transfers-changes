using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Projects.Commands.ConversionTasks
{
    public record UpdateHandoverTaskDataByProjectIdCommand(ProjectId ProjectId, bool? NotApplicable, bool? ReviewProjectInformation, bool? MakeNotes, bool? AttendHandoverMeeting) : IRequest<Result<Unit>>;

    public class UpdateHandoverTaskDataByProjectIdCommandHandler : IRequestHandler<UpdateHandoverTaskDataByProjectIdCommand, Result<Unit>>
    {
        private readonly ICompleteRepository<Project> _projectRepository;
        private readonly ICompleteRepository<ConversionTasksData> _conversionTaskRepository;

        public UpdateHandoverTaskDataByProjectIdCommandHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<ConversionTasksData> conversionTaskRepository)
        {
            _projectRepository = projectRepository;
            _conversionTaskRepository = conversionTaskRepository;
        }

        public async Task<Result<Unit>> Handle(UpdateHandoverTaskDataByProjectIdCommand request, CancellationToken cancellationToken)
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
}
