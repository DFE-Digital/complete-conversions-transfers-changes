using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters
{
    public class IncomingUkprnQuery(string? ukprn) : IQueryObject<Project>
    {
        public IQueryable<Project> Apply(IQueryable<Project> query) =>
            string.IsNullOrWhiteSpace(ukprn)
                ? query
                : query.Where(p => p.IncomingTrustUkprn != null
                                   && p.IncomingTrustUkprn.Value.ToString() == ukprn);
    }
}
