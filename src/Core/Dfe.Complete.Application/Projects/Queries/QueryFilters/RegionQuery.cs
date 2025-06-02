using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class RegionQuery(Region? r) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> q)
        => r.HasValue ? q.Where(p => p.Region == r.Value) : q;
}