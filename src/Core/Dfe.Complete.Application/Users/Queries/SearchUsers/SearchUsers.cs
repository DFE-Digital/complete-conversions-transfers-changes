using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;

namespace Dfe.Complete.Application.Users.Queries.SearchUsers
{ 
    public record SearchUsersQuery(
       string Email) : PaginatedRequest<PaginatedResult<List<User>>>;

    public class SearchUsersQueryHandler(
        ICompleteRepository<User> users)
        : IRequestHandler<SearchUsersQuery, PaginatedResult<List<User>>>
    {
        public async Task<PaginatedResult<List<User>>> Handle(SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var searchQuery = await users.FetchAsync(user => user.Email != null && user.Email.Contains(request.Email), cancellationToken);

                var itemCount = searchQuery.Count;

                var userList = searchQuery
                    .Skip(request.Page * request.Count).Take(request.Count).ToList(); 

                return PaginatedResult<List<User>>.Success(userList, itemCount);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<User>>.Failure(ex.Message);
            }
        }
    }
}
