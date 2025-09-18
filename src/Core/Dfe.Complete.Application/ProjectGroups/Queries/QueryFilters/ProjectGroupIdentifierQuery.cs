using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.ProjectGroups.Queries.QueryFilters;

public class ProjectGroupIdentifierQuery(string? projectId) : IQueryObject<ProjectGroup>
{
    public IQueryable<ProjectGroup> Apply(IQueryable<ProjectGroup> q)
        => projectId != null ? q.Where(p => p.GroupIdentifier == projectId) : q;
}
