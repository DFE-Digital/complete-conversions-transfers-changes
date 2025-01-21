using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Utils;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject
{
    public record CreateTransferProjectCommand(
        Urn Urn,
        Ukprn OutgoingTrustUkprn,
        Ukprn IncomingTrustUkprn,
        DateOnly SignificantDate,
        bool IsSignificantDateProvisional,
        bool IsDueTo2Ri,
        bool IsDueToInedaquateOfstedRating,
        bool IsDueToIssues,
        bool OutGoingTrustWillClose,
        DateOnly AdvisoryBoardDate,
        string AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string OutgoingTrustSharepointLink,
        string GroupReferenceNumber,
        string HandoverComments,
        string? UserAdId) : IRequest<ProjectId>;

    public class CreateTransferProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository)
        : IRequestHandler<CreateTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var projectUser = await projectRepository.GetUserByAdId(request.UserAdId, cancellationToken);
            var projectUserTeam = projectUser?.Team;
            var projectUserId = projectUser?.Id; 

            var projectTeam = EnumExtensions.FromDescription<ProjectTeam>(projectUserTeam);
            var region = EnumMapper.MapTeamToRegion(projectTeam);
            var regionCharValue = region.GetCharValue();
            
            var createdAt = DateTime.UtcNow;
            var transferTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);

            var groupId = await projectRepository.GetProjectGroupIdByIdentifierAsync(request.GroupReferenceNumber, cancellationToken);

            string team = projectTeam.ToDescription();
            DateTime? assignedAt = DateTime.UtcNow;
            UserId? projectUserAssignedToId = projectUserId;

            var project = Project.CreateTransferProject
                (projectId,
                    request.Urn,
                    createdAt,
                    createdAt,
                    TaskType.Transfer,
                    ProjectType.Transfer,
                    transferTaskId,
                    regionCharValue,
                    team,
                    projectUser?.Id,
                    projectUserAssignedToId,
                    assignedAt,
                    request.IncomingTrustUkprn,
                    request.OutgoingTrustUkprn,
                    groupId?.Value,
                    request.EstablishmentSharepointLink,
                    request.IncomingTrustSharepointLink,
                    request.OutgoingTrustSharepointLink,
                    request.AdvisoryBoardDate,
                    request.AdvisoryBoardConditions,
                    request.SignificantDate,
                    request.IsSignificantDateProvisional,
                    request.IsDueTo2Ri
                ); 
            
            if (!string.IsNullOrEmpty(request.HandoverComments))
            {
                project.Notes.Add(new Note
                {
                    Id = new NoteId(Guid.NewGuid()), CreatedAt = createdAt, Body = request.HandoverComments,
                    ProjectId = projectId, TaskIdentifier = "handover", UserId = projectUser?.Id
                });
            }
            
            await transferTaskRepository.AddAsync(transferTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}