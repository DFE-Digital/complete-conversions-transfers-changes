using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.TrustCache
{
    public class TrustCacheService(ITrustsV4Client trustsClient) : ITrustCache
    {
        private readonly Dictionary<Ukprn, TrustDto> UkprnCache = [];
        private readonly Dictionary<string, TrustDto> TrnCache = [];

        public async Task<TrustDto> GetTrustAsync(Ukprn ukprn)
        {
            if (UkprnCache.TryGetValue(ukprn, out TrustDto? value))
            {
                return value;
            }

            var trust = await trustsClient.GetTrustByUkprn2Async(ukprn.ToString());

            AddTrustToCache(trust);

            return trust!;
        }

        public async Task<TrustDto> GetTrustByTrnAsync(string trn)
        {
            if (TrnCache.TryGetValue(trn, out TrustDto? value))
            {
                return value;
            }

            var trust = await trustsClient.GetTrustByTrustReferenceNumberAsync(trn);

            AddTrustToCache(trust);

            return trust;
        }

        private void AddTrustToCache(TrustDto trust)
        {
            if (trust != null)
            {
                if (trust.Ukprn != null)
                {
                    UkprnCache.TryAdd(trust.Ukprn, trust);
                    UkprnCache[trust.Ukprn!] = trust;
                }
                if (trust.ReferenceNumber != null)
                {
                    TrnCache.TryAdd(trust.ReferenceNumber, trust);
                    TrnCache[trust.ReferenceNumber!] = trust;
                }
            }
        }

        public async Task HydrateCache(IEnumerable<Ukprn> ukprns)
        {
            if (ukprns == null || !ukprns.Any())
            {
                return;
            }
            var trusts = await trustsClient.GetByUkprnsAllAsync(ukprns.Select(ukprn => ukprn.ToString()));

            foreach (var trust in trusts)
            {
                AddTrustToCache(trust);
            }
        }
    }
}
