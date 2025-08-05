using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateHandoverProjectCommand(
        ProjectId ProjectId,
        string SchoolSharepointLink,
        string IncomingTrustSharepointLink,
        string? OutgoingTrustSharepointLink,
        bool AssignedToRegionalCaseworkerTeam,
        string? HandoverComments,
        UserId UserId,
        ProjectTeam UserTeam,
        bool? TwoRequiresImprovement
    ) : IRequest<Result<bool>>;

    public class UpdateHandoverProjectCommandHandler(
        ICompleteRepository<Project> projectRepository)
        : IRequestHandler<UpdateHandoverProjectCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateHandoverProjectCommand request,
            CancellationToken cancellationToken)
        {
            var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken)
                ?? throw new NotFoundException($"Project with {request.ProjectId} is not found", "ProjectId");

            project.EstablishmentSharepointLink = request.SchoolSharepointLink;
            project.IncomingTrustSharepointLink = request.IncomingTrustSharepointLink;
            project.OutgoingTrustSharepointLink = request.OutgoingTrustSharepointLink;
            project.State = ProjectState.Active;
            project.TwoRequiresImprovement = request.TwoRequiresImprovement;

            AssignedToRegionalCaseworkerTeam(request, project);

            AddNoteWhenHandoverCommentsPresent(project, request);

            await projectRepository.UpdateAsync(project, cancellationToken);
            return Result<bool>.Success(true);
        }

        private static void AssignedToRegionalCaseworkerTeam(UpdateHandoverProjectCommand request, Project project)
        {
            project.Team = request.AssignedToRegionalCaseworkerTeam ? ProjectTeam.RegionalCaseWorkerServices : request.UserTeam;
            project.AssignedToId = request.AssignedToRegionalCaseworkerTeam ? null : request.UserId;
            project.AssignedAt = DateTime.UtcNow;
        }

        private static void AddNoteWhenHandoverCommentsPresent(Project project, UpdateHandoverProjectCommand request)
        {
            var dateTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.HandoverComments))
            {
                project.Notes.Add(new Note()
                {
                    Id = new NoteId(Guid.NewGuid()),
                    ProjectId = request.ProjectId,
                    UserId = request.UserId,
                    Body = request.HandoverComments!,
                    TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(),
                    CreatedAt = dateTime,
                    UpdatedAt = dateTime
                });
            }
        }
    }
}
