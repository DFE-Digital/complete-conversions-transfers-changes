using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Users.Queries.SearchUsers
{
    public record SearchUsersQuery(
        string Query,
        bool FilterToAssignableUsers = false) : PaginatedRequest<PaginatedResult<List<User>>>;

    public class SearchUsersQueryHandler(
        IUserReadRepository userReadRepository)
        : IRequestHandler<SearchUsersQuery, PaginatedResult<List<User>>>
    {
        public async Task<PaginatedResult<List<User>>> Handle(SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var assignableUsersQuery = request.FilterToAssignableUsers
                    ? new AssignableUserQuery().Apply(userReadRepository.Users)
                    : userReadRepository.Users;

                var searchQuery = assignableUsersQuery.Where(
                    user => string.IsNullOrEmpty(request.Query) ||
                            EF.Functions.Like(user.FirstName + " " + user.LastName + " " + user.Email,
                                $"%{request.Query}%"));

                var itemCount = await searchQuery.CountAsync(cancellationToken);

                var userList = await searchQuery
                    .Skip(request.Page * request.Count).Take(request.Count).ToListAsync(cancellationToken);

                return PaginatedResult<List<User>>.Success(userList, itemCount);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<User>>.Failure(ex.Message);
            }
        }
    }
}