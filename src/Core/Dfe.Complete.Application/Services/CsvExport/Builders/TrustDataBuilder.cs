using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class TrustDataBuilder<T>(ITrustCache trustCache, Func<T, Ukprn> ukprn, Func<TrustDetailsDto, string> selector) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var x = trustCache.GetTrustAsync(ukprn(value)).Result;
            
            if(x == null)
                return string.Empty;

            return selector(x);
        }
    }
}
