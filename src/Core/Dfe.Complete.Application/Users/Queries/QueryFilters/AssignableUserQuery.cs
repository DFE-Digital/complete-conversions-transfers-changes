using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Queries.QueryFilters
{
    public class AssignableUserQuery() : IQueryObject<User>
    {
        public IQueryable<User> Apply(IQueryable<User> q)
            => q.Where(u => u.Team != null && u.Team != "data_consumers" && u.Team != "business_support" && u.Team != "service_support");
    }
}
