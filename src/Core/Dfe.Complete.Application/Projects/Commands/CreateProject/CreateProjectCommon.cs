using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Common;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateProjectCommonResult(
    GetLocalAuthorityBySchoolUrnResponseDto LocalAuthority,
    Region Region,
    ProjectGroupDto? ProjectGroupDto,
    UserDto CreatedByUser,
    UserDto? AssignedUser,
    ProjectTeam ProjectTeam,
    DateTime CreatedAt,
    DateTime? AssignedAt,
    ProjectId ProjectId
);

public record CreateProjectCommonCommand(
    Urn Urn,
    string? GroupReferenceNumber,
    bool HandingOverToRegionalCaseworkService,
    string? UserAdId);

public class CreateProjectCommon(
    ICompleteRepository<GiasEstablishment> establishmentRepository,
    ISender sender) : ICreateProjectCommon
{
    public async Task<CreateProjectCommonResult>  CreateCommonProject(CreateProjectCommonCommand request,
        CancellationToken cancellationToken)
    {
        var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
            cancellationToken);

        if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
            throw new NotFoundException(
                $"No Local authority could be found via Establishments for School Urn: {request.Urn.Value}.",
                nameof(request.Urn), innerException: new Exception(localAuthorityIdRequest.Error));

        var establishment =
            await establishmentRepository.FindAsync(giasEstablishment => giasEstablishment.Urn == request.Urn,
                cancellationToken);

        if (establishment is null)
        {
            throw new NotFoundException($"No establishment could be found for Urn: {request.Urn.Value}.",
                nameof(request.Urn));
        }

        var region = establishment.RegionCode?.ToEnumFromChar<Region>();

        if (region is null)
        {
            throw new NotFoundException($"No region could be found for establishment with Urn: {request.Urn.Value}.",
                nameof(request.Urn));
        }

        var createdAt = DateTime.UtcNow;
        var projectId = new ProjectId(Guid.NewGuid());

        ProjectGroupDto? projectGroupDto = null;
        if (!string.IsNullOrEmpty(request.GroupReferenceNumber))
        {
            var projectGroupRequest =
                await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(request.GroupReferenceNumber),
                    cancellationToken);

            if (!projectGroupRequest.IsSuccess)
                throw new NotFoundException($"Project Group retrieval failed", nameof(request.GroupReferenceNumber),
                    new Exception(projectGroupRequest.Error));

            projectGroupDto = projectGroupRequest.Value ?? throw new NotFoundException(
                $"No Project Group found with reference number: {request.GroupReferenceNumber}",
                nameof(request.GroupReferenceNumber));
        }

        Result<UserDto?>? userRequest = null;
        
        if (!string.IsNullOrEmpty(request.UserAdId))
            userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

        if (userRequest is not { IsSuccess: true } || userRequest.Value is null)
            throw new NotFoundException("No user found.", innerException: new Exception(userRequest?.Error));

        var createdByUser = userRequest.Value;
        
        UserDto? assignedUser = null;
        DateTime? assignedAt = null;
        ProjectTeam team;

        if (request.HandingOverToRegionalCaseworkService)
        {
            team = ProjectTeam.RegionalCaseWorkerServices;
        }
        else
        {
            assignedUser = userRequest.Value;
            assignedAt = createdAt;
            team = (assignedUser?.Team).FromDescription<ProjectTeam>();
        }

        return new CreateProjectCommonResult(localAuthorityIdRequest.Value, region.Value,
            projectGroupDto, createdByUser, assignedUser, team, createdAt, assignedAt, projectId);
    }
}