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
    public record UpdateTransferProjectCommand(
        ProjectId ProjectId,
        Ukprn IncomingTrustUkprn,
        Ukprn OutgoingTrustUkprn,
        string? NewTrustReferenceNumber,
        string? GroupReferenceNumber,
        DateOnly AdvisoryBoardDate,
        string? AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string OutgoingTrustSharepointLink,
        bool TwoRequiresImprovement,
        bool InadequateOfsted,
        bool FinancialSafeguardingGovernanceIssues,
        bool OutgoingTrustToClose,
        bool IsHandingToRCS,
        string? HandoverComments,
        UserDto User
    ) : IRequest;

    public class UpdateTransferProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICompleteRepository<TransferTasksData> transferTaskDataRepository,
        ICompleteRepository<Note> noteRepository
    ) : IRequestHandler<UpdateTransferProjectCommand>
    {
        public async Task Handle(UpdateTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.FindAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project == null)
            {
                return;
            }

            project.IncomingTrustUkprn = request.IncomingTrustUkprn;
            project.OutgoingTrustUkprn = request.OutgoingTrustUkprn;
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
            project.OutgoingTrustSharepointLink = request.OutgoingTrustSharepointLink;

            project.TwoRequiresImprovement = request.TwoRequiresImprovement;

            var tasksData = await transferTaskDataRepository.GetAsync(p => p.Id == project.TasksDataId);
            if (tasksData != null) // we take for granted that each project has a TasksData record attached
            {
                tasksData.InadequateOfsted = request.InadequateOfsted;
                tasksData.FinancialSafeguardingGovernanceIssues = request.FinancialSafeguardingGovernanceIssues;
                tasksData.OutgoingTrustToClose = request.OutgoingTrustToClose;

                await transferTaskDataRepository.UpdateAsync(tasksData, cancellationToken);
            }

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


            await projectRepository.UpdateAsync(project, cancellationToken);
        }
    }
}
