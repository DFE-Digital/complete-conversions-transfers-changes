using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class NewTrustReferenceQuery(string? r) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> q)
        => string.IsNullOrWhiteSpace(r)
            ? q
            : q.Where(p => p.NewTrustReferenceNumber == r);
}