using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Users.Queries.ListAllUsers;

public record ListAllUsersInTeamWithProjectsQuery(ProjectTeam Team)
    : PaginatedRequest<PaginatedResult<List<UserWithProjectsDto>>>;

public class ListAllUsersInTeamWithProjectsHandler(ICompleteRepository<User> usersRepository, ILogger<ListAllUsersInTeamWithProjectsHandler> logger)
    : IRequestHandler<ListAllUsersInTeamWithProjectsQuery, PaginatedResult<List<UserWithProjectsDto>>>
{
    public async Task<PaginatedResult<List<UserWithProjectsDto>>> Handle(ListAllUsersInTeamWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usersQuery = usersRepository.Query()
                .Include(u => u.ProjectAssignedTos)
                .Where(u => u.Team == request.Team.ToDescription())
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            var usersWithProjects = await usersQuery
                .Paginate(request.Page, request.Count)
                .Select(user => new UserWithProjectsDto(
                    user.Id,
                    user.FullName,
                    user.Email,
                    request.Team,
                    new List<ListAllProjectsResultModel>(),
                    user.ProjectAssignedTos.Count(p => p.Type == ProjectType.Conversion),
                    user.ProjectAssignedTos.Count(p => p.Type == ProjectType.Transfer)
                ))
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<UserWithProjectsDto>>.Success(usersWithProjects, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllUsersInTeamWithProjectsHandler), request);
            return PaginatedResult<List<UserWithProjectsDto>>.Failure(ex.Message);
        }
    }
}
