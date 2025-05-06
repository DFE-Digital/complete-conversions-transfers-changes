using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Users.Queries.ListAllUsers;

public record ListAllUsersWithProjectsQuery(
    ProjectState? State,
    ProjectTeam? Team = null,
    bool? UserHasProjects = true
)
    : PaginatedRequest<PaginatedResult<List<ListAllUsersWithProjectsResultModel>>>;

public class ListAllUsersWithProjectsHandler(ICompleteRepository<User> users)
    : IRequestHandler<ListAllUsersWithProjectsQuery, PaginatedResult<List<ListAllUsersWithProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllUsersWithProjectsResultModel>>> Handle(ListAllUsersWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var filteredUsers = users.Query();

            if (request.Team is not null)
                filteredUsers = filteredUsers.Where(u => u.Team == request.Team.ToDescription());

            if (request.UserHasProjects == true)
                filteredUsers = filteredUsers.Where(u =>
                    u.ProjectAssignedTos.Any(pa =>
                        request.State == null || pa.State == request.State));

            var count = await filteredUsers.CountAsync(cancellationToken);

            var result = await filteredUsers
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.LastName)
                .Paginate(request.Page, request.Count)
                .Select(user => new ListAllUsersWithProjectsResultModel(
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Team.FromDescriptionValue<ProjectTeam>(),
                    user.ProjectAssignedTos
                        .Where(project => request.State == null || project.State == request.State)
                        .Count(project => project.Type == ProjectType.Conversion),
                    user.ProjectAssignedTos
                        .Where(project => request.State == null || project.State == request.State)
                        .Count(project => project.Type == ProjectType.Transfer)
                ))
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<ListAllUsersWithProjectsResultModel>>.Success(result, count);
        }
        catch (Exception ex)
        {
            return PaginatedResult<List<ListAllUsersWithProjectsResultModel>>.Failure(ex.Message);
        }
    }
}