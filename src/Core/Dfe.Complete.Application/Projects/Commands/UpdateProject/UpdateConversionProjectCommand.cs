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
        ProjectGroupId? GroupIdentifier,
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
        ICompleteRepository<Note> noteRepository
    ) : IRequestHandler<UpdateConversionProjectCommand>
    {
        public async Task Handle(UpdateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.Query().FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project == null)
            {
                return;
            }

            project.IncomingTrustUkprn = request.IncomingTrustUkprn;
            project.NewTrustReferenceNumber = request.NewTrustReferenceNumber;
            project.GroupId = request.GroupIdentifier;
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
                project.Team = (request.User.Team).FromDescription<ProjectTeam>();
            }

            var userId = new UserId(request.User.Id.Value);

            var lastComment = await noteRepository.Query()
                .Where(x => x.ProjectId == project.Id && x.UserId == userId && x.TaskIdentifier == NoteTaskIdentifier.Handover.ToDescription())
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastComment != null) // we have a comment in database for this project, user and type
            {
                if (!string.IsNullOrEmpty(request.HandoverComments))
                {
                    lastComment.Body = request.HandoverComments;
                    await noteRepository.UpdateAsync(lastComment, cancellationToken);
                }
                else
                {
                    project.RemoveNote(lastComment.Id);
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
