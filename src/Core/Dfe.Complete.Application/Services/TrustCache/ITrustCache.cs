using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.TrustCache
{
    public interface ITrustCache
    {
        Task<TrustDto> GetTrustAsync(Ukprn ukprn);
        Task<TrustDto> GetTrustByTrnAsync(string trn);

        Task HydrateCache(IEnumerable<Ukprn> ukprns);
    }
}
