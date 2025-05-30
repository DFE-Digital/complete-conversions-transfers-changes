using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class StateQuery(ProjectState? state) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> query) =>
        state.HasValue
            ? query.Where(p => p.State == state.Value)
            : query;
}