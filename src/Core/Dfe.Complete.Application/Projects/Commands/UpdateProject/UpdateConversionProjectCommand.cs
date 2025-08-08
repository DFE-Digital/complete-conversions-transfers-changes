using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateConversionProjectCommand(
        ProjectId ProjectId,
        Ukprn IncomingTrustUkprn,
        string? NewTrustReferenceNumber,
        string? GroupReferenceNumber,
        DateOnly AdvisoryBoardDate,
        string? AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        bool IsHandingToRCS,
        string? HandoverComments,
        bool DirectiveAcademyOrder,
        bool TwoRequiresImprovement,
        UserDto User
    ) : IRequest;

    public class UpdateConversionProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ProjectGroup> projectGroupRepository
    ) : IRequestHandler<UpdateConversionProjectCommand>
    {
        public async Task Handle(UpdateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.Query()
            // TODO ideally expose a repository for this instead of leaking infra into application layer
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
            if (project == null)
            {
                return;
            }

            project.IncomingTrustUkprn = request.IncomingTrustUkprn;
            project.NewTrustReferenceNumber = request.NewTrustReferenceNumber;

            if (!string.IsNullOrEmpty(request.GroupReferenceNumber))
            {
                var group = projectGroupRepository.FindAsync(x => x.GroupIdentifier == request.GroupReferenceNumber, cancellationToken);
                if (group != null)
                {
                    project.GroupId = group.Result?.Id;
                }
            }
            else
            {
                project.GroupId = null;
            }

            project.AdvisoryBoardDate = request.AdvisoryBoardDate;
            project.AdvisoryBoardConditions = request.AdvisoryBoardConditions;
            project.EstablishmentSharepointLink = request.EstablishmentSharepointLink;
            project.IncomingTrustSharepointLink = request.IncomingTrustSharepointLink;

            if (request.IsHandingToRCS)
            {
                project.Team = ProjectTeam.RegionalCaseWorkerServices;
            }
            else
            {
                project.AssignedToId = request.User.Id;
                project.AssignedAt = DateTime.UtcNow;
                project.Team = request.User.Team.FromDescription<ProjectTeam>();
            }

            var userId = new UserId(request.User.Id.Value);

            var lastComment = project.Notes
                .Where(x => x.UserId == userId && x.TaskIdentifier == NoteTaskIdentifier.Handover.ToDescription())
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (lastComment != null) // we have a comment in database for this project, user and type
            {
                if (string.IsNullOrEmpty(request.HandoverComments))
                {
                    project.RemoveNote(lastComment.Id);
                }
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

            project.DirectiveAcademyOrder = request.DirectiveAcademyOrder;
            project.TwoRequiresImprovement = request.TwoRequiresImprovement;

            await projectRepository.UpdateAsync(project, cancellationToken);
        }
    }
}
