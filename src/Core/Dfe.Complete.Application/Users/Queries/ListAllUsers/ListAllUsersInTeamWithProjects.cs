using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Users.Queries.ListAllUsers;

public record ListAllUsersInTeamWithProjectsQuery(ProjectTeam? Team)
    : PaginatedRequest<PaginatedResult<List<UserWithProjectsDto>>>;

public class ListAllUsersInTeamWithProjectsHandler(ICompleteRepository<User> usersRepository, ICompleteRepository<Project> projectsRepository)
    : IRequestHandler<ListAllUsersInTeamWithProjectsQuery, PaginatedResult<List<UserWithProjectsDto>>>
{
    public async Task<PaginatedResult<List<UserWithProjectsDto>>> Handle(ListAllUsersInTeamWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usersQuery = usersRepository.Query()
                .Where(u => u.Team == request.Team.ToDescription())
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            var users = await usersQuery
                .Paginate(request.Page, request.Count)
                .ToListAsync(cancellationToken);

            var usersWithProjects = users
                .Select(user => new UserWithProjectsDto(
                    user.Id,
                    user.FullName,
                    user.Email,
                    request.Team,
                    null,
                    projectsRepository.Query().Count(p => p.AssignedToId == user.Id && p.Type == ProjectType.Conversion),
                    projectsRepository.Query().Count(p => p.AssignedToId == user.Id && p.Type == ProjectType.Transfer)
                )).ToList();

            return PaginatedResult<List<UserWithProjectsDto>>.Success(usersWithProjects, totalCount);
        }
        catch (Exception ex)
        {
            return PaginatedResult<List<UserWithProjectsDto>>.Failure(ex.Message);
        }
    }
}