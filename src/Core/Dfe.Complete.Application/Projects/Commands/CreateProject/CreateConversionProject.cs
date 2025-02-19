using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetUser;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject
{
    public record CreateConversionProjectCommand(
        Urn Urn,
        DateOnly SignificantDate,
        bool IsSignificantDateProvisional,
        Ukprn IncomingTrustUkprn,
        bool IsDueTo2Ri,
        bool HasAcademyOrderBeenIssued,
        DateOnly AdvisoryBoardDate,
        string AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string GroupReferenceNumber,
        bool HandingOverToRegionalCaseworkService,
        string HandoverComments,
        string? UserAdId) : IRequest<ProjectId>;

    public class CreateConversionProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository,
        ISender sender)
        : IRequestHandler<CreateConversionProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotFoundException("lol");
            
            var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
                cancellationToken);

            if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
                throw new NotFoundException($"No Local authority could be found via Establishments for School Urn: {request.Urn.Value}.",
                    innerException: new Exception(localAuthorityIdRequest.Error));
            
            // The user Team should be moved as a Claim or Group to the Entra (MS AD)
            var userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

            if (!userRequest.IsSuccess)
                throw new NotFoundException("No user found.", innerException: new Exception(userRequest.Error));
            
            var projectUser = userRequest.Value;

            var projectUserTeam = projectUser?.Team;
            var projectUserId = projectUser?.Id;

            var projectTeam = EnumExtensions.FromDescription<ProjectTeam>(projectUserTeam);
            var region = EnumMapper.MapTeamToRegion(projectTeam);

            var createdAt = DateTime.UtcNow;
            var conversionTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);

            var projectGroupRequest =
                await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(request.GroupReferenceNumber),
                    cancellationToken);

            if (!projectGroupRequest.IsSuccess || projectGroupRequest.Value == null)
                throw new NotFoundException($"Project Group retrieval failed: {projectGroupRequest.Error}");

            var groupId = projectGroupRequest.Value?.Id;

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

            var project = Project.CreateConversionProject(
                projectId,
                request.Urn,
                createdAt,
                createdAt,
                TaskType.Conversion,
                ProjectType.Conversion,
                conversionTaskId,
                request.SignificantDate,
                request.IsSignificantDateProvisional,
                request.IncomingTrustUkprn,
                region,
                request.IsDueTo2Ri,
                request.HasAcademyOrderBeenIssued,
                request.AdvisoryBoardDate,
                request.AdvisoryBoardConditions,
                request.EstablishmentSharepointLink,
                request.IncomingTrustSharepointLink,
                groupId,
                team,
                projectUser?.Id,
                projectUserAssignedToId,
                assignedAt,
                request.HandoverComments, 
                localAuthorityIdRequest.Value.LocalAuthorityId.Value);

            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}