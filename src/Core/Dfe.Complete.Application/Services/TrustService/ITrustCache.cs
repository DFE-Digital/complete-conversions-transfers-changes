using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.TrustService
{
    public interface ITrustCache
    {
        Task<TrustDetailsDto> GetTrustAsync(Ukprn ukprn);
        Task<TrustDetailsDto> GetTrustByTrnAsync(string trn);

        Task HydrateCache(IEnumerable<Ukprn> ukprn);
    }
}
