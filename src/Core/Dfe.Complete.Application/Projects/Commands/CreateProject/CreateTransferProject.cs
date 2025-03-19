using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetUser;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

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
    bool HandingOverToRegionalCaseworkService,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    string OutgoingTrustSharepointLink,
    string? GroupReferenceNumber,
    string HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

public class CreateTransferProjectCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository,  
    ISender sender)
    : IRequestHandler<CreateTransferProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateTransferProjectCommand request, CancellationToken cancellationToken)
    {
        var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
            cancellationToken);

        if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
            throw new NotFoundException($"No Local authority could be found via Establishments for School Urn: {request.Urn.Value}.", nameof(request.Urn), innerException: new Exception(localAuthorityIdRequest.Error));
            
        Result<UserDto?>? userRequest = null;
            
        if (!string.IsNullOrEmpty(request.UserAdId))
            userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

        if (userRequest is not { IsSuccess: true })
            throw new NotFoundException("No user found.", innerException: new Exception(userRequest?.Error));
        
        var projectUser = userRequest.Value;

        var projectUserTeam = projectUser?.Team;
        var projectUserId = projectUser?.Id; 

        var projectTeam = projectUserTeam.FromDescription<ProjectTeam>();
        var region = EnumMapper.MapTeamToRegion(projectTeam);

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var projectId = new ProjectId(Guid.NewGuid());

        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);

        ProjectGroupDto? projectGroupDto = null;
        if (!string.IsNullOrEmpty(request.GroupReferenceNumber))
        {
            var projectGroupRequest = await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(request.GroupReferenceNumber), cancellationToken);

            if (!projectGroupRequest.IsSuccess)
                throw new NotFoundException("Project Group retrieval failed", nameof(request.GroupReferenceNumber), new Exception(projectGroupRequest.Error));

            projectGroupDto = projectGroupRequest.Value ?? throw new NotFoundException($"No Project Group found with reference number: {request.GroupReferenceNumber}", nameof(request.GroupReferenceNumber));
        }
            
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
            projectGroupDto?.Id,
            request.EstablishmentSharepointLink,
            request.IncomingTrustSharepointLink,
            request.OutgoingTrustSharepointLink,
            request.AdvisoryBoardDate,
            request.AdvisoryBoardConditions,
            request.SignificantDate,
            request.IsSignificantDateProvisional,
            request.IsDueTo2Ri, 
            request.HandoverComments, 
            localAuthorityIdRequest.Value.LocalAuthorityId.Value
        ); 
            
        await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }
        
}