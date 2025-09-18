using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Application.ProjectGroups.Queries.QueryFilters;
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
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

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
    string? UserAdId,
    int? IncomingTrustUkprn = null
    );

public class CreateProjectCommon(
    IMapper mapper,
    ICompleteRepository<GiasEstablishment> establishmentRepository,
    IProjectGroupReadRepository projectGroupReadRepository,
    IProjectGroupWriteRepository projectGroupWriteRepository,
    ISender sender) : ICreateProjectCommon
{
    protected async Task<GetLocalAuthorityBySchoolUrnResponseDto> GetLocalAuthorityForUrn(int urn, CancellationToken cancellationToken)
    {
        var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(urn),
            cancellationToken);

        if (!localAuthorityIdRequest.IsSuccess
            || localAuthorityIdRequest.Value?.LocalAuthorityId == null
            || localAuthorityIdRequest.Value.LocalAuthorityId == Guid.Empty)
            throw new NotFoundException(
                $"No Local authority could be found via Establishments for School Urn: {urn}.",
                nameof(urn), innerException: new Exception(localAuthorityIdRequest.Error));

        return localAuthorityIdRequest.Value;
    }

    protected async Task<Region> GetRegionForUrn(int urn, CancellationToken cancellationToken)
    {
        var establishment =
            await establishmentRepository.FindAsync(giasEstablishment => giasEstablishment.Urn == new Urn(urn),
                cancellationToken) ?? throw new NotFoundException($"No establishment could be found for Urn: {urn}.",
                nameof(urn));
        var region = (establishment.RegionCode?.ToEnumFromChar<Region>()) ?? throw new NotFoundException($"No region could be found for establishment with Urn: {urn}.",
                nameof(urn));
        return region;
    }

    protected async Task<ProjectGroupDto?> GetOrCreateProjectGroup(string? groupReferenceNumber, int? incomingTrustUkprn, CancellationToken cancellationToken)
    {
        // No group reference number provided
        if (string.IsNullOrEmpty(groupReferenceNumber))
            return null;

        // Must have an incoming trust UKPRN to validate against
        if (incomingTrustUkprn == null)
            throw new Exception("An Incoming Trust UKPRN must be provided when a Group Reference Number is specified.");

        var projectGroup = await new ProjectGroupIdentifierQuery(groupReferenceNumber)
            .Apply(projectGroupReadRepository.ProjectGroups)
            .ProjectTo<ProjectGroupDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        // Group found
        if (projectGroup != null)
        {
            // Ensure that the request UKPRN matches the group
            if (projectGroup.TrustUkprn!.Value != incomingTrustUkprn)
                throw new Exception(
                    $"The provided Incoming Trust UKPRN {incomingTrustUkprn} does not match the Incoming Trust UKPRN {projectGroup.TrustUkprn.Value} associated with the Group Reference Number {groupReferenceNumber}.");

            // Otherwise, return the group
            return projectGroup;
        }

        // No existing group found, create a new group
        var newProjectGroupDto = new ProjectGroupDto
        {
            Id = new ProjectGroupId(Guid.NewGuid()),
            GroupIdentifier = groupReferenceNumber,
            TrustUkprn = new Ukprn(incomingTrustUkprn.Value),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await projectGroupWriteRepository.CreateProjectGroupAsync(mapper.Map<ProjectGroup>(newProjectGroupDto), cancellationToken);
        return newProjectGroupDto;
    }

    public async Task<CreateProjectCommonResult> CreateCommonProject(CreateProjectCommonCommand request,
        CancellationToken cancellationToken)
    {
        var localAuthority = await GetLocalAuthorityForUrn(request.Urn.Value, cancellationToken);
        var region = await GetRegionForUrn(request.Urn.Value, cancellationToken);

        var createdAt = DateTime.UtcNow;
        var projectId = new ProjectId(Guid.NewGuid());

        var projectGroupDto = await GetOrCreateProjectGroup(request.GroupReferenceNumber, request.IncomingTrustUkprn, cancellationToken);

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

        return new CreateProjectCommonResult(localAuthority, region,
            projectGroupDto, createdByUser, assignedUser, team, createdAt, assignedAt, projectId);
    }
}