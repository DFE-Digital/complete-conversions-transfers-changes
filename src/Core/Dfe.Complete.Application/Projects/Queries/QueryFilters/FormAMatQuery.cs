using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class FormAMatQuery(bool? isForm) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> query)
    {
        if (isForm.HasValue) return isForm.Value
                ? query.Where(p => p.NewTrustReferenceNumber != null)
                : query.Where(p => p.NewTrustReferenceNumber == null);

        return query;
    }
}