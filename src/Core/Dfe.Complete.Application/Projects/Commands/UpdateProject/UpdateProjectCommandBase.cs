using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public abstract class UpdateProjectCommandBase<TRequest> where TRequest : IUpdateProjectRequest
    {
        protected readonly ICompleteRepository<Project> ProjectRepository;
        protected readonly ICompleteRepository<ProjectGroup> ProjectGroupRepository;

        protected UpdateProjectCommandBase(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<ProjectGroup> projectGroupRepository)
        {
            ProjectRepository = projectRepository;
            ProjectGroupRepository = projectGroupRepository;
        }

        public async Task Handle(TRequest request, CancellationToken cancellationToken)
        {
            var project = await ProjectRepository.Query()
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
            
            if (project == null)
                return;

            await UpdateCommonProjectProperties(project, request, cancellationToken);
            await UpdateSpecificProjectProperties(project, request, cancellationToken);
            UpdateTeamAssignment(project, request);
            UpdateHandoverComments(project, request);

            await ProjectRepository.UpdateAsync(project, cancellationToken);
        }

        protected async Task UpdateCommonProjectProperties(Project project, TRequest request, CancellationToken cancellationToken)
        {
            project.IncomingTrustUkprn = request.IncomingTrustUkprn;
            project.NewTrustReferenceNumber = request.NewTrustReferenceNumber;

            // Handle group assignment
            if (!string.IsNullOrEmpty(request.GroupReferenceNumber))
            {
                var group = await ProjectGroupRepository.FindAsync(x => x.GroupIdentifier == request.GroupReferenceNumber, cancellationToken);
                if (group != null)
                    project.GroupId = group.Id;
            }
            else
                project.GroupId = null;

            project.AdvisoryBoardDate = request.AdvisoryBoardDate;
            project.AdvisoryBoardConditions = request.AdvisoryBoardConditions;
            project.EstablishmentSharepointLink = request.EstablishmentSharepointLink;
            project.IncomingTrustSharepointLink = request.IncomingTrustSharepointLink;
            project.TwoRequiresImprovement = request.TwoRequiresImprovement;
        }

        protected static void UpdateTeamAssignment(Project project, TRequest request)
        {
            if (request.IsHandingToRCS)
                project.Team = ProjectTeam.RegionalCaseWorkerServices;
            else
            {
                project.AssignedToId = request.User.Id;
                project.AssignedAt = DateTime.UtcNow;
                project.Team = request.User.Team.FromDescription<ProjectTeam>();
            }
        }

        protected void UpdateHandoverComments(Project project, TRequest request)
        {
            var userId = new UserId(request.User.Id.Value);

            var lastComment = project.Notes
                .Where(x => x.UserId == userId && x.TaskIdentifier == NoteTaskIdentifier.Handover.ToDescription())
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (lastComment != null) // we have a comment in database for this project, user and type
            {
                if (string.IsNullOrEmpty(request.HandoverComments))
                    project.RemoveNote(lastComment.Id);
                else
                {
                    lastComment.Body = request.HandoverComments;
                    project.UpdateNote(lastComment);
                }
            }
            else if (!string.IsNullOrEmpty(request.HandoverComments)) // there is no current comment and we want to add a comment
            {
                project.AddNote(new Note
                {
                    CreatedAt = project.CreatedAt,
                    ProjectId = project.Id,
                    Body = request.HandoverComments,
                    TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(),
                    UserId = userId
                });
            }
        }

        // Abstract method for project-specific updates
        protected abstract Task UpdateSpecificProjectProperties(Project project, TRequest request, CancellationToken cancellationToken);
    }
}
