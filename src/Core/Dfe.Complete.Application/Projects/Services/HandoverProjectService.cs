
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Services;

public class HandoverProjectService(
    ISender sender,
    ICompleteRepository<Project> projectRepository,
    IProjectGroupWriteRepository projectGroupWriteRepository,
    ICompleteRepository<ConversionTasksData> conversionTaskRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository) : IHandoverProjectService
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
            .Apply(new TypeQuery(ProjectType.Conversion)
            .Apply(projectRepository.Query().AsNoTracking())))
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
            throw new NotFoundException(
                $"No Local authority could be found via Establishments for School Urn: {urn}.",
                nameof(urn), innerException: new Exception(localAuthorityIdRequest.Error));

        return localAuthorityIdRequest.Value.LocalAuthorityId.Value;
    }

    public async Task<Region> GetRegionForUrn(int urn, CancellationToken cancellationToken)
    {
        var establishment = await sender.Send(new GetGiasEstablishmentByUrnQuery(new Urn(urn)), cancellationToken);
        if (!establishment.IsSuccess || establishment.Value == null)
            throw new NotFoundException($"No establishment could be found for Urn: {urn}.");
        var region = (establishment.Value.RegionCode?.ToEnumFromChar<Region>()) ?? throw new NotFoundException($"No region could be found for establishment with Urn: {urn}.",
                nameof(urn));
        return region;
    }

    public async Task<ProjectGroupDto?> GetGroupForGroupId(string? groupId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(groupId))
            return null;

        var projectGroupRequest = await sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(groupId), cancellationToken);

        if (!projectGroupRequest.IsSuccess)
            throw new NotFoundException($"Project Group retrieval failed", nameof(groupId),
                new Exception(projectGroupRequest.Error));

        return projectGroupRequest.Value;
    }

    public async Task<ProjectGroupId> CreateProjectGroup(string groupId, int incomingTrustUkprn, CancellationToken cancellationToken)
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