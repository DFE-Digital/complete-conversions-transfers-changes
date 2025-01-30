using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Models;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateMatTransferProjectCommand(
    Urn Urn,
    string NewTrustName,
    string NewTrustReferenceNumber,
    Ukprn OutgoingTrustUkprn,
    DateOnly SignificantDate,
    bool IsSignificantDateProvisional,
    bool IsDueTo2Ri,
    bool IsDueToInedaquateOfstedRating,
    bool IsDueToIssues,
    bool HandingOverToRegionalCaseworkService,
    bool OutGoingTrustWillClose,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    string OutgoingTrustSharepointLink,
    string HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

public class CreateMatTransferProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository,
        ICompleteRepository<User> userRepository)
        : IRequestHandler<CreateMatTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateMatTransferProjectCommand request, CancellationToken cancellationToken)
        {
            // The user Team should be moved as a Claim or Group to the Entra (MS AD)
            var projectUser = await GetUserByAdId(request.UserAdId);

            var projectUserTeam = projectUser.Team;
            var projectUserId = projectUser?.Id;

            var projectTeam = EnumExtensions.FromDescription<ProjectTeam>(projectUserTeam);
            var region = EnumMapper.MapTeamToRegion(projectTeam);

            var createdAt = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var transferTask = new TransferTasksData(new TaskDataId(tasksDataId), createdAt, createdAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);
            
            ProjectTeam team;
            DateTime? assignedAt = null;
            UserId? projectUserAssignedToId = null;

            if (request.HandingOverToRegionalCaseworkService)
            {
                team = ProjectTeam.RegionalCaseWorkerServices;
            }
            else
            {
                team = projectTeam;
                assignedAt = DateTime.UtcNow;
                projectUserAssignedToId = projectUserId;
            }

            var project = Project.CreateMatTransferProject(
                    projectId,
                    request.Urn,
                    createdAt,
                    updatedAt: createdAt,
                    request.OutgoingTrustUkprn,
                    TaskType.Transfer,
                    ProjectType.Transfer,
                    tasksDataId,
                    region,
                    team,
                    projectUser?.Id,
                    projectUserAssignedToId,
                    assignedAt,
                    request.EstablishmentSharepointLink,
                    request.IncomingTrustSharepointLink,
                    request.OutgoingTrustSharepointLink,
                    request.AdvisoryBoardDate,
                    request.AdvisoryBoardConditions,
                    request.SignificantDate,
                    request.IsSignificantDateProvisional,
                    request.IsDueTo2Ri,
                    request.NewTrustName,
                    request.NewTrustReferenceNumber,
                    request.HandoverComments);


        await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }

        private async Task<User> GetUserByAdId(string? userAdId) => await userRepository.FindAsync(x => x.ActiveDirectoryUserId == userAdId);
    }