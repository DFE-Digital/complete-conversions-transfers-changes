using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters
{
    public class ProjectUrnQuery(Urn? urn) : IQueryObject<Project>
    {
        public IQueryable<Project> Apply(IQueryable<Project> q)
            => urn != null ? q.Where(p => p.Urn == urn) : q;
    }
}
