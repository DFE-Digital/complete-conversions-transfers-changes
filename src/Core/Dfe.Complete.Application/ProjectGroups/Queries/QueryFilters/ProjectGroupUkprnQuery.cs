using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.ProjectGroups.Queries.QueryFilters
{
    public class ProjectGroupUkprnQuery(Ukprn? ukprn) : IQueryObject<ProjectGroup>
    {
        public IQueryable<ProjectGroup> Apply(IQueryable<ProjectGroup> q)
            => ukprn != null ? q.Where(p => p.TrustUkprn == ukprn) : q;
    }
}
