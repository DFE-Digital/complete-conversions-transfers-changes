using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Queries.QueryFilters
{
    public class AssignableUserQuery() : IQueryObject<User>
    {
        public IQueryable<User> Apply(IQueryable<User> q)
        {
            // Define assignable teams using Description values from ProjectTeam enum
            var assignableTeamDescriptions = new[] {
                "regional_casework_services",
                "london",
                "south_east",
                "yorkshire_and_the_humber",
                "north_west",
                "east_of_england",
                "west_midlands",
                "north_east",
                "south_west",
                "east_midlands"
            };

            return q.Where(u => !string.IsNullOrEmpty(u.Team) &&
                               assignableTeamDescriptions.Any(team => u.Team.Contains(team)));
        }
    }
}
