using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class IncomingTrustDataBuilder<T>(ITrustCache trustCache, Func<T, Project> projectSelector, Func<TrustDto, string> TrustSelector) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var project = projectSelector(value);

            TrustDto trust;

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

            var selection = TrustSelector(trust);

            return selection ?? "";
        }
    }
}
