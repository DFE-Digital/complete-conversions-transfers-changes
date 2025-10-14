using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Users.Queries.QueryFilters
{
    public class UserIdQuery(UserId id) : IQueryObject<User>
    {
        public IQueryable<User> Apply(IQueryable<User> q)
            => id != null ? q.Where(u => u.Id == id) : q;
    }
}
