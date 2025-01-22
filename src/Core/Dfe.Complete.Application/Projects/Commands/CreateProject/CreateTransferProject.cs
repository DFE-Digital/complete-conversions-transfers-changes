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
        ICompleteRepository<TransferTasksData> transferTaskRepository,  
        ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICompleteRepository<User> userRepository)
        : IRequestHandler<CreateTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var projectUser = await GetUserByAdId(request.UserAdId);
            var projectUserTeam = projectUser?.Team;
            var projectUserId = projectUser?.Id; 

            var projectTeam = EnumExtensions.FromDescription<ProjectTeam>(projectUserTeam);
            var region = EnumMapper.MapTeamToRegion(projectTeam);

            var createdAt = DateTime.UtcNow;
            var transferTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);

            var groupId = await GetProjectGroupIdByIdentifierAsync(request.GroupReferenceNumber);

            ProjectTeam team = projectTeam;
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
                    region,
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
                    request.IsDueTo2Ri, 
                    request.HandoverComments
                ); 
            
            await transferTaskRepository.AddAsync(transferTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
        
        private async Task<User> GetUserByAdId(string? userAdId) => await userRepository.FindAsync(x => x.ActiveDirectoryUserId == userAdId);

        private async Task<ProjectGroupId> GetProjectGroupIdByIdentifierAsync(string groupReferenceNumber) => (await projectGroupRepository.FindAsync(x => x.GroupIdentifier == groupReferenceNumber)).Id;
    }
}