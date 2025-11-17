using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Queries.QueryFilters
{
    public class ActiveTeamLeadersQuery : IQueryObject<User>
    {
        public IQueryable<User> Apply(IQueryable<User> q)
            => q.Where(u => u.ManageTeam == true);
    }
}

