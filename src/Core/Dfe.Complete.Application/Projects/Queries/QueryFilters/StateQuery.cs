using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class StateQuery(List<ProjectState>? states) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> query)
        => states != null && states.Count != 0
            ? query.Where(p => states.Contains(p.State))
            : query;
}
