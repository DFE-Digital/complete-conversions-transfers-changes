using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.ProjectGroups.Interfaces;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverConversionProjectCommand(
    [Required] int? Urn,
    [Required] int? IncomingTrustUkprn,
    [Required] DateOnly? AdvisoryBoardDate,
    [Required] DateOnly? ProvisionalConversionDate,
    [Required] string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required] bool? DirectiveAcademyOrder,
    string? AdvisoryBoardConditions,
    string? GroupId = null) : IRequest<ProjectId>;

// TODO use query pattern not ICompleteRepository
public class CreateHandoverConversionProjectCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository,
    IProjectGroupWriteRepository projectGroupWriteRepository,
    ISender sender)
    : IRequestHandler<CreateHandoverConversionProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        await ValidateRequest(request, cancellationToken);

        var projectId = new ProjectId(Guid.NewGuid());
        var now = DateTime.UtcNow;
        var urn = request.Urn!.Value;
        var localAuthorityId = await GetLocalAuthorityForUrn(urn, cancellationToken);
        var region = await GetRegionForUrn(urn, cancellationToken);
        var group = await GetGroupForGroupId(request.GroupId, cancellationToken);

        if (group != null) ValidateGroupId(group, request.IncomingTrustUkprn!.Value);

        ProjectGroupId? groupId = null;
        if (group != null) groupId = group.Id;
        if (group == null && request.GroupId != null) groupId = await CreateProjectGroup(request.GroupId, request.IncomingTrustUkprn!.Value, cancellationToken);

        var user = await GetOrCreateUser(request, region, cancellationToken);

        // Create conversion task data
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), now, now);

        // Create the project in inactive state
        var project = Project.CreateConversionProject(
            projectId,
            new Urn(urn),
            now,
            now,
            TaskType.Conversion,
            ProjectType.Conversion,
            conversionTaskId,
            request.ProvisionalConversionDate!.Value,
            true,
            new Ukprn(request.IncomingTrustUkprn!.Value),
            region,
            false,
            request.DirectiveAcademyOrder ?? false,
            request.AdvisoryBoardDate!.Value,
            request.AdvisoryBoardConditions ?? "",
            string.Empty,
            string.Empty,
            groupId,
            null,
            user.Id,
            null,
            null,
            string.Empty,
            localAuthorityId);

        project.State = ProjectState.Inactive;

        if (request.DirectiveAcademyOrder!.Value)
        {
            project.DirectiveAcademyOrder = true;
        }

        project.PrepareId = request.PrepareId!.Value;

        await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }

    private async Task ValidateRequest(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
{
        // Pure validation logic should be in domain layer. Move on next ticket
        // Validate URN format (6 digits)
        if (request.Urn < 100000 || request.Urn > 999999)
            throw new ValidationException("URN must be a 6-digit integer");

        // Validate UKPRN format (8 digits)
        if (request.IncomingTrustUkprn < 10000000 || request.IncomingTrustUkprn > 19999999)
            throw new ValidationException("Incoming trust UKPRN must be an 8 digit integer beginning with 1");

        // Validate advisory board date is in the past
        if (request.AdvisoryBoardDate!.Value > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("Advisory board date must be in the past");

        // Validate provisional conversion date is first day of month
        if (request.ProvisionalConversionDate!.Value.Day != 1)
            throw new ValidationException("Provisional conversion date must be the first day of the month");

        // Validate email domain
        if (!request.CreatedByEmail.EndsWith("@education.gov.uk", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Created by email must be from @education.gov.uk domain");

        // Validate group ID format if provided
        if (!string.IsNullOrEmpty(request.GroupId))
        {
            var groupIdPattern = @"^GRP_\d{8}$";
            if (!Regex.IsMatch(request.GroupId, groupIdPattern))
                throw new ValidationException("Group ID must match format GRP_XXXXXXXX (8 digits)");
        }

        // Check if URN already exists in active/inactive conversion projects
        var existingProject = await new ProjectUrnQuery(new Urn((int)request.Urn!))
            .Apply(new StateQuery([ProjectState.Active, ProjectState.Inactive])
            .Apply(new TypeQuery(ProjectType.Conversion)
            .Apply(projectRepository.Query().AsNoTracking())))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingProject != null)
        {
            throw new ValidationException($"URN {request.Urn} already exists in active/inactive conversion projects");
        }
    }

    private static void ValidateGroupId(ProjectGroupDto group, int trustUkprn)
    {
        // Domain layer as pure logic
        if (group.TrustUkprn?.Value != trustUkprn)
            throw new ValidationException($"Trust UKPRN {trustUkprn} is not the same as the group UKPRN for group {group.GroupIdentifier}");
    }

    private async Task<User> GetOrCreateUser(CreateHandoverConversionProjectCommand request, Region region, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == request.CreatedByEmail, cancellationToken);

        if (existingUser != null)
            return existingUser;

        // Create new user
        var userId = new UserId(Guid.NewGuid());

        var newUser = User.Create(
            userId,
            request.CreatedByEmail,
            request.CreatedByFirstName,
            request.CreatedByLastName,
            region.ToDescription()
        );

        await userRepository.AddAsync(newUser, cancellationToken);
        return newUser;
    }

    protected async Task<Guid> GetLocalAuthorityForUrn(int urn, CancellationToken cancellationToken)
    {
        var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(urn),
            cancellationToken);

        if (!localAuthorityIdRequest.IsSuccess
            || localAuthorityIdRequest.Value?.LocalAuthorityId == null
            || localAuthorityIdRequest.Value.LocalAuthorityId == Guid.Empty)
            throw new NotFoundException(
                $"No Local authority could be found via Establishments for School Urn: {urn}.",
                nameof(urn), innerException: new Exception(localAuthorityIdRequest.Error));

        return localAuthorityIdRequest.Value.LocalAuthorityId.Value;
    }

    protected async Task<Region> GetRegionForUrn(int urn, CancellationToken cancellationToken)
    {
        var establishment = await sender.Send(new GetGiasEstablishmentByUrnQuery(new Urn(urn)), cancellationToken);

        if (!establishment.IsSuccess
            || establishment.Value == null)
            throw new NotFoundException($"No establishment could be found for Urn: {urn}.");

        var region = (establishment.Value.RegionCode?.ToEnumFromChar<Region>()) ?? throw new NotFoundException($"No region could be found for establishment with Urn: {urn}.",
                nameof(urn));
        return region;
    }

    protected async Task<ProjectGroupDto?> GetGroupForGroupId(string? groupId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(groupId))
            return null;

        var projectGroupRequest = await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(groupId), cancellationToken);

        if (!projectGroupRequest.IsSuccess)
            throw new NotFoundException($"Project Group retrieval failed", nameof(groupId),
                new Exception(projectGroupRequest.Error));

        return projectGroupRequest.Value;
    }

    protected async Task<ProjectGroupId> CreateProjectGroup(string groupId, int incomingTrustUkprn, CancellationToken cancellationToken)
    {
        // Consider merging the logic in Get/Create/Validate group with the logic in UpdateProjectCommandBase - move to application service
        var id = new ProjectGroupId(Guid.NewGuid());
        var createdGroup = new ProjectGroup
        {
            Id = id,
            GroupIdentifier = groupId,
            TrustUkprn = new Ukprn(incomingTrustUkprn),
        };
        await projectGroupWriteRepository.CreateProjectGroupAsync(createdGroup, cancellationToken);
        return id;
    }
}

// Test cases
// Empty body
// No LA found
// No group found
// Group found back ukprn
// Group found good ukprn
// No group provided - void groupid
// Happy path