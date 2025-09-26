using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Queries.QueryFilters
{
    public class UserEmailQuery(string email) : IQueryObject<User>
    {
        public IQueryable<User> Apply(IQueryable<User> q)
            => !string.IsNullOrWhiteSpace(email) ? q.Where(u => u.Email == email) : q;
    }
}
