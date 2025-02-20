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

public record ListAllUsersWithProjectsQuery(ProjectState? State)
    : PaginatedRequest<PaginatedResult<List<UserWithProjectsDto>>>;

public class ListAllUsersWithProjectsHandler(ICompleteRepository<User> users)
    : IRequestHandler<ListAllUsersWithProjectsQuery, PaginatedResult<List<UserWithProjectsDto>>>
{
    public async Task<PaginatedResult<List<UserWithProjectsDto>>> Handle(ListAllUsersWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var filteredUsers = users.Query()
                .Where(user => user.ProjectAssignedTos.Any(project => request.State == null || project.State == request.State));
            
            var count = await filteredUsers.CountAsync(cancellationToken);
            
            var userProjectData = filteredUsers
                .Select(user => new
                {
                    User = user,
                    FilteredProjects = user.ProjectAssignedTos
                        .Where(project => request.State == null || project.State == request.State)
                });
            
            var result = await userProjectData
                .OrderBy(u => u.User.FirstName)
                .ThenBy(u => u.User.LastName)
                .Paginate(request.Page, request.Count)
                .Select(u => new UserWithProjectsDto(
                    u.User.Id,
                    $"{u.User.FirstName} {u.User.LastName}",
                    u.User.Email,
                    u.User.Team.FromDescriptionValue<ProjectTeam>(),
                    u.FilteredProjects.Select(project => new ListAllProjectsResultModel(
                        null,
                        project.Id,
                        project.Urn,
                        project.SignificantDate,
                        project.State,
                        project.Type,
                        project.FormAMat,
                        null
                    )).ToList(),
                    u.FilteredProjects.Count(project => project.Type == ProjectType.Conversion),
                    u.FilteredProjects.Count(project => project.Type == ProjectType.Transfer)
                ))
                .ToListAsync(cancellationToken);
            
            return PaginatedResult<List<UserWithProjectsDto>>.Success(result, count);
        }
        catch (Exception ex)
        {
            return PaginatedResult<List<UserWithProjectsDto>>.Failure(ex.Message);
        }
    }
}