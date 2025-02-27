using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
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
        IEstablishmentsV4Client establishmentsClient,
        ISender sender)
        : IRequestHandler<CreateMatTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateMatTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
                cancellationToken);

            if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
                throw new NotFoundException($"No Local authority could be found via Establishments for School Urn: {request.Urn.Value}.", nameof(request.Urn), innerException: new Exception(localAuthorityIdRequest.Error));
            
            
            var region = (await establishmentsClient.GetEstablishmentByUrnAsync(request.Urn.Value.ToString(),
                cancellationToken)).Gor?.Code?.ToEnumFromChar<Region>();

            var createdAt = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var transferTask = new TransferTasksData(new TaskDataId(tasksDataId), createdAt, createdAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);
            
            ProjectTeam team;
            DateTime? assignedAt = null;
            UserId? projectUserAssignedToId = null;            
            UserDto? projectUser = null;

            if (request.HandingOverToRegionalCaseworkService)
            {
                team = ProjectTeam.RegionalCaseWorkerServices;
            }
            else
            {
                if (request.UserAdId is null)
                    throw new ArgumentException(
                        "Project cannot be unassigned if it is not being handed over to Regional Case Worker Services");
                // The user Team should be moved as a Claim or Group to the Entra (MS AD)
                var userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

                if (!userRequest.IsSuccess || userRequest.Value == null)
                    throw new NotFoundException("No user found.", innerException: new Exception(userRequest.Error));
            
                projectUser = userRequest.Value;

                var projectUserTeam = projectUser.Team;
                var projectUserId = projectUser.Id;

                var projectTeam = projectUserTeam.FromDescription<ProjectTeam>();
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
                    projectUser.Id,
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