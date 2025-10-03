
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Application.Users.Queries.GetUser;
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
    ICompleteRepository<ConversionTasksData> conversionTaskRepository) : IHandoverProjectService
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
            throw new Exception($"Failed to create user with email {userDto.Email}: {newUser.Error}");

        return newUser.Value;
    }

    public ConversionTasksData CreateConversionTaskAsync()
    {
        var conversionTaskId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        return new ConversionTasksData(new TaskDataId(conversionTaskId), now, now);
    }

    public async Task<Project?> FindExistingProjectAsync(int urn, CancellationToken cancellationToken)
    {
        return await new ProjectUrnQuery(new Urn(urn))
            .Apply(new StateQuery([ProjectState.Active, ProjectState.Inactive])
            .Apply(new TypeQuery(ProjectType.Conversion)
            .Apply(projectRepository.Query().AsNoTracking())))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveProjectAndTaskAsync(Project project, ConversionTasksData conversionTask, CancellationToken cancellationToken)
    {
        await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);
    }
}