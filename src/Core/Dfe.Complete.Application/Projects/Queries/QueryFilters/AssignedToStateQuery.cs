using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class AssignedToStateQuery(AssignedToState? m) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> q)
        => m switch
        {
            AssignedToState.AssignedOnly => q.Where(p => p.AssignedToId != null),
            AssignedToState.UnassignedOnly => q.Where(p => p.AssignedToId == null),
            _ => q
        };
}