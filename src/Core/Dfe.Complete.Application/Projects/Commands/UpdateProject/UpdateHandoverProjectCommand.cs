using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        ICompleteRepository<Project> projectRepository,
        IKeyContactReadRepository keyContactReadRepo,
        IKeyContactWriteRepository keyContactWriteRepository,
        ILogger<UpdateHandoverProjectCommandHandler> logger)
        : IRequestHandler<UpdateHandoverProjectCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateHandoverProjectCommand request,
            CancellationToken cancellationToken)
        {
            var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken)
                ?? throw new NotFoundException($"Project with {request.ProjectId} is not found", "ProjectId");

            var dateTime = DateTime.UtcNow;
            project.EstablishmentSharepointLink = request.SchoolSharepointLink;
            project.IncomingTrustSharepointLink = request.IncomingTrustSharepointLink;
            project.OutgoingTrustSharepointLink = request.OutgoingTrustSharepointLink;
            project.State = ProjectState.Active;
            project.TwoRequiresImprovement = request.TwoRequiresImprovement;
            project.UpdatedAt = dateTime;

            AssignedToRegionalCaseworkerTeam(request, project, dateTime);

            AddNoteWhenHandoverCommentsPresent(project, request, dateTime);

            await AddKeyContactIfDoesNotExist(project, dateTime, cancellationToken);

            await projectRepository.UpdateAsync(project, cancellationToken);

            return Result<bool>.Success(true);
        }
        private async Task AddKeyContactIfDoesNotExist(Project project, DateTime dateTime, CancellationToken cancellationToken)
        {
            var keycontact = await keyContactReadRepo.KeyContacts.FirstOrDefaultAsync(k => k.ProjectId == project.Id, cancellationToken);
            if (keycontact != null)
            {
                logger.LogError("Key contact already exists for handover project {ProjectId}", project.Id);
                return;
            }
            await keyContactWriteRepository.AddKeyContactAsync(new KeyContact
            {
                Id = new KeyContactId(Guid.NewGuid()),
                ProjectId = project.Id,
                UpdatedAt = dateTime,
                CreatedAt = dateTime,
            }, cancellationToken);
        }

        private static void AssignedToRegionalCaseworkerTeam(UpdateHandoverProjectCommand request, Project project, DateTime dateTime)
        {
            var wasAssignedToRegionalTeam = project.Team == ProjectTeam.RegionalCaseWorkerServices;
            var isAssignedToRegionalTeam = request.AssignedToRegionalCaseworkerTeam;

            project.Team = isAssignedToRegionalTeam ? ProjectTeam.RegionalCaseWorkerServices : request.UserTeam;
            project.AssignedToId = isAssignedToRegionalTeam ? null : request.UserId;
            project.AssignedAt = dateTime;

            // Raise event if project is newly assigned to regional team
            if (isAssignedToRegionalTeam && !wasAssignedToRegionalTeam)
            {
                var schoolName = project.GiasEstablishment?.Name ?? $"School URN {project.Urn.Value}";
                project.RaiseProjectAssignedToRegionalTeamEvent(
                    project.Id.Value.ToString(),
                    schoolName);
            }
        }

        private static void AddNoteWhenHandoverCommentsPresent(Project project, UpdateHandoverProjectCommand request, DateTime dateTime)
        {

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
