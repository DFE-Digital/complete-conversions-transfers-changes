using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class IncomingTrustDataBuilder<T>(ITrustCache trustCache, Func<T, Project> projectSelector, Func<TrustDetailsDto, string> TrustSelector) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var project = projectSelector(value);

            TrustDetailsDto trust;

            if(project.IncomingTrustUkprn == null)
            {
                trust = trustCache.GetTrustByTrnAsync(project.NewTrustReferenceNumber).Result;
            }
            else
            {
                trust = trustCache.GetTrustAsync(project.IncomingTrustUkprn).Result;
            }

            if(trust == null)
                return string.Empty;

            return TrustSelector(trust);
        }
    }
}
