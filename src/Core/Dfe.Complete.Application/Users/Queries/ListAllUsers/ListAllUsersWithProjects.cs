using Dfe.Complete.Application.Common;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Users.Queries.ListAllUsers;

public record ListAllUsersWithProjectsQuery(ProjectState? State = ProjectState.Active)
    : PaginatedRequest<PaginatedResult<List<UserWithProjectsResultModel>>>;

public class ListAllUsersWithProjectsHandler(ICompleteRepository<User> users)
    : IRequestHandler<ListAllUsersWithProjectsQuery, PaginatedResult<List<UserWithProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<UserWithProjectsResultModel>>> Handle(ListAllUsersWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await users.Query().Where(user =>
                    user.ProjectAssignedTos.Count(project => request.State == null || project.State == request.State) >
                    0)
                .CountAsync(cancellationToken);
            var result = await users.Query().Where(user =>
                    user.ProjectAssignedTos.Count(project => request.State == null || project.State == request.State) >
                    0)
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.LastName)
                .Skip(request.Count * request.Page).Take(request.Count)
                .Select(user =>
                    new UserWithProjectsResultModel(
                        user.Id,
                        $"{user.FirstName} {user.LastName}",
                        user.Email,
                        EnumExtensions.FromDescription<ProjectTeam>(user.Team),
                        user.ProjectAssignedTos
                            .Where(project => request.State == null || project.State == request.State).Select(project =>
                                new ListAllProjectsResultModel(
                                    null,
                                    project.Id,
                                    project.Urn,
                                    project.SignificantDate,
                                    project.State,
                                    project.Type,
                                    true,
                                    null
                                )).ToList(),
                        user.ProjectAssignedTos
                            .Where(project => request.State == null || project.State == request.State)
                            .Count(project => project.Type == ProjectType.Conversion),
                        user.ProjectAssignedTos
                            .Where(project => request.State == null || project.State == request.State)
                            .Count(project => project.Type == ProjectType.Transfer)
                    )
                )
                .ToListAsync(cancellationToken);
            return PaginatedResult<List<UserWithProjectsResultModel>>.Success(result, count);
        }
        catch (Exception ex)
        {
            return PaginatedResult<List<UserWithProjectsResultModel>>.Failure(ex.Message);
        }
    }
}