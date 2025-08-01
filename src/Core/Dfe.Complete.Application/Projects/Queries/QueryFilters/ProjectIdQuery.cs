using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters
{
    public class ProjectIdQuery(ProjectId? projectId) : IQueryObject<Project>
    {
        public IQueryable<Project> Apply(IQueryable<Project> q)
            => projectId != null ? q.Where(p => p.Id == projectId) : q;
    }
}
