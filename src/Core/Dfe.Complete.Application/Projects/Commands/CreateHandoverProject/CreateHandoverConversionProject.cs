using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Domain.Validators;
using Microsoft.Extensions.Logging;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Constants;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverConversionProjectCommand(
    [Required]
    [Urn]
    int? Urn,

    [Required]
    [Ukprn]
    int? IncomingTrustUkprn,

    [Required]
    [PastDate (AllowToday = true)]
    DateOnly? AdvisoryBoardDate,

    [Required]
    [FirstOfMonthDate]
    DateOnly? ProvisionalConversionDate,

    [Required]
    [InternalEmail]
    string CreatedByEmail,

    [Required] string CreatedByFirstName,

    [Required] string CreatedByLastName,

    [Required] int? PrepareId,

    [Required] bool? DirectiveAcademyOrder,

    string? AdvisoryBoardConditions,

    [GroupReferenceNumber]
    string? GroupId = null) : IRequest<ProjectId>;

public class CreateHandoverConversionProjectCommandHandler(
    IUnitOfWork unitOfWork,
    ITrustsV4Client trustClient,
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository,
    IProjectGroupWriteRepository projectGroupWriteRepository,
    ISender sender,
    ILogger<CreateHandoverConversionProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverConversionProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

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
            if (group == null && !string.IsNullOrWhiteSpace(request.GroupId)) groupId = await CreateProjectGroup(request.GroupId, request.IncomingTrustUkprn!.Value, cancellationToken);

            var user = await GetOrCreateUser(request, region, cancellationToken);

            // Create conversion task data
            var conversionTaskId = Guid.NewGuid();
            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), now, now);

            var project = Project.CreateHandoverConversionProject(
                projectId,
                new Urn(urn),
                conversionTaskId,
                request.ProvisionalConversionDate!.Value,
                new Ukprn(request.IncomingTrustUkprn!.Value),
                region,
                request.DirectiveAcademyOrder ?? false,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                groupId,
                user.Id,
                localAuthorityId);

            project.PrepareId = request.PrepareId!.Value;

            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating handover conversion project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover conversion project for URN: {request.Urn}", ex);
        }
    }

    private async Task ValidateRequest(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
    {
        // Check if URN already exists in active/inactive conversion projects
        var existingProject = await new ProjectUrnQuery(new Urn((int)request.Urn!))
            .Apply(new StateQuery([ProjectState.Active, ProjectState.Inactive])
            .Apply(new TypeQuery(ProjectType.Conversion)
            .Apply(projectRepository.Query().AsNoTracking())))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingProject != null)
            throw new ValidationException(string.Format(ValidationConstants.UrnExistsValidationMessage, request.Urn));

        var existingTrust = await trustClient.GetTrustByUkprn2Async(request.IncomingTrustUkprn!.Value.ToString(), cancellationToken);

        if (existingTrust == null)
            throw new ValidationException(ValidationConstants.NoTrustFoundValidationMessage);
    }

    private static void ValidateGroupId(ProjectGroupDto group, int trustUkprn)
    {
        if (group.TrustUkprn?.Value != trustUkprn)
            throw new ValidationException(string.Format(ValidationConstants.MismatchedTrustInGroupValidationMessage, trustUkprn, group.GroupIdentifier));
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
        if (!establishment.IsSuccess || establishment.Value == null)
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
