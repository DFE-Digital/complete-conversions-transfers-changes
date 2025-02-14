using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
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
        ISender sender)
        : IRequestHandler<CreateMatTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateMatTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
                cancellationToken);

            if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
                throw new Exception($"Failed to retrieve Local authority for School URN: {request.Urn}");
            
            // The user Team should be moved as a Claim or Group to the Entra (MS AD)
            var userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId));

            if (!userRequest.IsSuccess)
                throw new Exception($"User retrieval failed: {userRequest.Error}");

            var projectUser = userRequest.Value;

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
                    request.HandoverComments,
                    localAuthorityIdRequest.Value.LocalAuthorityId.Value);
            
        await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }
}