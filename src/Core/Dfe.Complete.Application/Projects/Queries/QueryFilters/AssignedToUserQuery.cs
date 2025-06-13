using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class AssignedToUserQuery(UserId? u) : IQueryObject<Project>
{
    private readonly Guid? _userId = u?.Value;

    public IQueryable<Project> Apply(IQueryable<Project> q)
        => _userId.HasValue
            ? q.Where(p =>
                p.AssignedToId != null
                && p.AssignedToId.Value == _userId.Value)
            : q;
}