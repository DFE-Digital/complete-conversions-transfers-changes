using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class TypeQuery(ProjectType? t) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> q)
        => t.HasValue ? q.Where(p => p.Type == t.Value) : q;
}