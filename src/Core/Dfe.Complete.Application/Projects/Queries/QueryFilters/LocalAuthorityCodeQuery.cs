using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters
{
    public class LocalAuthorityCodeQuery(string? code) : IQueryObject<Project>
    {
        public IQueryable<Project> Apply(IQueryable<Project> q)
        {
            if (string.IsNullOrWhiteSpace(code)) return q;
            return q.Where(p =>
                p.GiasEstablishment != null &&
                p.GiasEstablishment.LocalAuthorityCode == code);
        }
    }
}
