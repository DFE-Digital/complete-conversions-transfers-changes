using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;

namespace Dfe.Complete.Application.Handover.Commands;

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
    ICompleteRepository<LocalAuthority> localAuthorityRepository)
    : IRequestHandler<CreateHandoverConversionProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        await ValidateRequest(request, cancellationToken);

        var projectId = new ProjectId(Guid.NewGuid());
        var now = DateTime.UtcNow;

        // Find or create the user based on email
        var user = await GetOrCreateUser(request, now, cancellationToken);

        // Get local authority for the URN (you may need to implement this lookup)
        var localAuthority = await GetLocalAuthorityForUrn(new Urn(request.Urn!.Value), cancellationToken);

        // Parse group ID if provided
        ProjectGroupId? groupId = null;
        if (!string.IsNullOrEmpty(request.GroupId))
        {
            await ValidateGroupId(request.GroupId, new Ukprn(request.IncomingTrustUkprn!.Value), cancellationToken);
            groupId = new ProjectGroupId(Guid.NewGuid()); // You may need to implement group lookup
        }

        // Create conversion task data
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), now, now);

        // Create the project in inactive state
        var project = Project.CreateConversionProject(
            projectId,
            new Urn(request.Urn!.Value),
            now,
            now,
            TaskType.Conversion,
            ProjectType.Conversion,
            conversionTaskId,
            request.ProvisionalConversionDate!.Value,
            false, // Not provisional since we have a specific date
            new Ukprn(request.IncomingTrustUkprn!.Value),
            null, // TODO check against ruby
            false, // IsDueTo2Ri - not specified in handover version
            false, // HasAcademyOrderBeenIssued - use DirectiveAcademyOrder instead
            request.AdvisoryBoardDate!.Value,
            request.AdvisoryBoardConditions,
            string.Empty, // EstablishmentSharepointLink - not required in lightweight
            string.Empty, // IncomingTrustSharepointLink - not required in lightweight
            groupId,
            null, // TODO check against ruby
            user.Id,
            null, // No assigned user initially
            null, // No assigned date initially
            string.Empty, // No handover comments
            localAuthority.Id.Value);

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
        // Validate URN format (6 digits)
        if (request.Urn < 100000 || request.Urn > 999999)
            throw new ValidationException("URN must be a 6-digit integer");

        // Validate UKPRN format (8 digits)
        if (request.IncomingTrustUkprn < 10000000 || request.IncomingTrustUkprn > 19999999)
            throw new ValidationException("Incoming trust UKPRN must be an 8 digit integer beginning with 1");

        // Validate advisory board date is in the past
        if (request.AdvisoryBoardDate.Value > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("Advisory board date must be in the past");

        // Validate provisional conversion date is first day of month
        if (request.ProvisionalConversionDate.Value.Day != 1)
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
            .Apply(new StateQuery([ProjectState.Active])
            .Apply(new TypeQuery(ProjectType.Conversion)
            .Apply(projectRepository.Query().AsNoTracking())))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingProject != null)
        {
            throw new ValidationException($"URN {request.Urn} already exists in active/inactive conversion projects");
        }
    }

    private async Task<User> GetOrCreateUser(CreateHandoverConversionProjectCommand request, DateTime now, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == request.CreatedByEmail, cancellationToken);

        if (existingUser != null)
        {
            return existingUser;
        }

        // Create new user
        var userId = new UserId(Guid.NewGuid());
        var newUser = User.Create(
            userId,
            request.CreatedByEmail,
            now,
            now,
            false, // ManageTeam
            false, // AddNewProject
            request.CreatedByFirstName,
            request.CreatedByLastName,
            Guid.NewGuid().ToString(), // ActiveDirectoryUserId - generate or get from actual AD
            false, // AssignToProject
            false, // ManageUserAccounts
            string.Empty, // ActiveDirectoryUserGroupIds
            null,
            null, // DeactivatedAt
            false, // ManageConversionUrns
            false, // ManageLocalAuthorities
            null // LatestSession
        );

        await userRepository.AddAsync(newUser, cancellationToken);
        return newUser;
    }

    private async Task<LocalAuthority> GetLocalAuthorityForUrn(Urn urn, CancellationToken cancellationToken)
    {
        // This is a placeholder - you'll need to implement URN to Local Authority lookup
        // This might involve calling the Academies API or having a local mapping
        var defaultLocalAuthority = await localAuthorityRepository.Query()
            .FirstOrDefaultAsync(cancellationToken);

        if (defaultLocalAuthority == null)
        {
            throw new InvalidOperationException("No local authority found");
        }

        return defaultLocalAuthority;
    }

    private async Task ValidateGroupId(string groupId, Ukprn trustUkprn, CancellationToken cancellationToken)
    {
        // Check if group exists and trust UKPRN matches other projects in the group
        var existingProjectsInGroup = await projectRepository.Query()
            .Where(p => p.GroupId!.Value.ToString() == groupId)
            .ToListAsync(cancellationToken);

        if (existingProjectsInGroup.Any())
        {
            var differentUkprns = existingProjectsInGroup
                .Where(p => p.IncomingTrustUkprn != trustUkprn)
                .Any();

            if (differentUkprns)
            {
                throw new ValidationException($"Trust UKPRN {trustUkprn.Value} does not match other projects in group {groupId}");
            }
        }
    }
}
