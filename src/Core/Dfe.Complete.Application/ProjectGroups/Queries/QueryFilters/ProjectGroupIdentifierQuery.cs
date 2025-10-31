using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.ProjectGroups.Queries.QueryFilters;

public class ProjectGroupIdentifierQuery(string? projectGroupIdentifier) : IQueryObject<ProjectGroup>
{
    public IQueryable<ProjectGroup> Apply(IQueryable<ProjectGroup> q)
        => projectGroupIdentifier != null ? q.Where(p => p.GroupIdentifier == projectGroupIdentifier) : q;
}
