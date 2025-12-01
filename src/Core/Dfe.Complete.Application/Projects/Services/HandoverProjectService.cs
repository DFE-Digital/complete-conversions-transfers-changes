using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.ProjectGroups.Commands;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Services;

public record HandoverProjectCommonData(
    ProjectId ProjectId,
    int Urn,
    Guid LocalAuthorityId,
    Region Region,
    UserId UserId);

public class HandoverProjectService(
    ISender sender,
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository,
    IKeyContactWriteRepository keyContactWriteRepository) : IHandoverProjectService
{
    public async Task<UserId> GetOrCreateUserAsync(UserDto userDto, CancellationToken cancellationToken)
    {
        var existingUser = await sender.Send(new GetUserByEmailQuery(userDto.Email!), cancellationToken);

        if (existingUser.IsSuccess && existingUser.Value != null)
            return existingUser.Value.Id;

        var newUser = await sender.Send(new CreateUserCommand(
            userDto.FirstName!,
            userDto.LastName!,
            userDto.Email!,
            userDto.Team.FromDescription<ProjectTeam>()), cancellationToken);

        if (!newUser.IsSuccess || newUser.Value == null)
            throw new UnknownException(string.Format(ErrorMessagesConstants.ExceptionCreatingUser, userDto.Email));

        return newUser.Value;
    }

    public ConversionTasksData CreateConversionTaskAsync()
    {
        var conversionTaskId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        return new ConversionTasksData(new TaskDataId(conversionTaskId), now, now);
    }

    public TransferTasksData CreateTransferTaskAsync(
        bool InadequateOfsted = false,
        bool FinancialSafeguardingGovernanceIssues = false,

        bool OutgoingTrustToClose = false
    )
    {
        var transferTaskId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        return new TransferTasksData(
            new TaskDataId(transferTaskId),
            now,
            now,
            InadequateOfsted,
            FinancialSafeguardingGovernanceIssues,
            OutgoingTrustToClose
        );
    }

    public async Task<Project?> FindExistingProjectAsync(int urn, CancellationToken cancellationToken)
    {
        return await new ProjectUrnQuery(new Urn(urn))
            .Apply(new StateQuery([ProjectState.Active, ProjectState.Inactive])
            .Apply(projectRepository.Query().AsNoTracking()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveProjectAndTaskAsync<TTaskData>(Project project, TTaskData taskData, CancellationToken cancellationToken)
        where TTaskData : class
    {
        if (taskData is ConversionTasksData conversionTask)
            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
        else if (taskData is TransferTasksData transferTask)
            await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        else
            throw new ArgumentException($"Unsupported task data type: {typeof(TTaskData).Name}", nameof(taskData));

        var now = DateTime.UtcNow;
        await keyContactWriteRepository.AddKeyContactAsync(new KeyContact
        {
            Id = new KeyContactId(Guid.NewGuid()),
            ProjectId = project.Id,
            UpdatedAt = now,
            CreatedAt = now,
        }, cancellationToken);

        await projectRepository.AddAsync(project, cancellationToken);
    }

    public void ValidateGroupId(ProjectGroupDto group, int trustUkprn)
    {
        if (group.TrustUkprn?.Value != trustUkprn)
            throw new ValidationException(string.Format(Constants.ValidationConstants.MismatchedTrustInGroupValidationMessage, trustUkprn, group.GroupIdentifier));
    }

    public async Task<Guid> GetLocalAuthorityForUrn(int urn, CancellationToken cancellationToken)
    {
        var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(urn),
            cancellationToken);

        if (!localAuthorityIdRequest.IsSuccess
            || localAuthorityIdRequest.Value?.LocalAuthorityId == null
            || localAuthorityIdRequest.Value.LocalAuthorityId == Guid.Empty)
            throw new UnprocessableContentException(
                $"No Local authority could be found via Establishments for School Urn: {urn}.",
                nameof(urn), innerException: new Exception(localAuthorityIdRequest.Error));

        return localAuthorityIdRequest.Value.LocalAuthorityId.Value;
    }

    public async Task<Region> GetRegionForUrn(int urn, CancellationToken cancellationToken)
    {
        var establishment = await sender.Send(new GetGiasEstablishmentByUrnQuery(new Urn(urn)), cancellationToken);
        if (!establishment.IsSuccess || establishment.Value == null)
            throw new UnprocessableContentException($"No establishment could be found for Urn: {urn}.");
        var region = (establishment.Value.RegionCode?.ToEnumFromChar<Region>()) ?? throw new UnprocessableContentException($"No region could be found for establishment with Urn: {urn}.",
                nameof(urn));
        return region;
    }

    public async Task<ProjectGroupId> GetOrCreateProjectGroup(string groupId, int incomingTrustUkprn, CancellationToken cancellationToken)
    {
        var projectGroupRequest = await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(groupId), cancellationToken);

        if (!projectGroupRequest.IsSuccess)
            throw new NotFoundException($"Project Group retrieval failed", nameof(groupId),
                new Exception(projectGroupRequest.Error));

        if (projectGroupRequest.Value != null)
        {
            ValidateGroupId(projectGroupRequest.Value, incomingTrustUkprn);
            return projectGroupRequest.Value.Id;
        }

        var createdGroup = await sender.Send(new CreateProjectGroupCommand(groupId, new Ukprn(incomingTrustUkprn)), cancellationToken);
        if (!createdGroup.IsSuccess || createdGroup.Value == null)
            throw new UnknownException($"Could not create project group: {createdGroup.Error}");

        return createdGroup.Value;
    }

    public async Task ValidateUrnAsync(int urn, CancellationToken cancellationToken)
    {
        // Check if URN already exists in active/inactive projects
        var existingProject = await FindExistingProjectAsync(urn, cancellationToken);
        if (existingProject != null)
            throw new UnprocessableContentException(string.Format(Constants.ValidationConstants.UrnExistsValidationMessage, urn));
    }

    public async Task ValidateTrustAsync(int trustUkprn, CancellationToken cancellationToken)
    {
        // Validate trust exists
        var trustResponse = await sender.Send(new GetTrustByUkprnRequest(trustUkprn.ToString()), cancellationToken)
            ?? throw new UnprocessableContentException(string.Format(Constants.ValidationConstants.NoTrustFoundValidationMessage, trustUkprn));

        if (!trustResponse.IsSuccess || trustResponse.Value == null)
            throw new UnprocessableContentException(string.Format(Constants.ValidationConstants.NoTrustFoundValidationMessage, trustUkprn.ToString()));

    }

    public async Task<HandoverProjectCommonData> PrepareCommonProjectDataAsync(int urn, string createdByFirstName, string createdByLastName, string createdByEmail, CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(Guid.NewGuid());
        var localAuthorityId = await GetLocalAuthorityForUrn(urn, cancellationToken);
        var region = await GetRegionForUrn(urn, cancellationToken);

        var userDto = new UserDto
        {
            FirstName = createdByFirstName,
            LastName = createdByLastName,
            Email = createdByEmail,
            Team = region.ToDescription()
        };
        var userId = await GetOrCreateUserAsync(userDto, cancellationToken);

        return new HandoverProjectCommonData(projectId, urn, localAuthorityId, region, userId);
    }
}