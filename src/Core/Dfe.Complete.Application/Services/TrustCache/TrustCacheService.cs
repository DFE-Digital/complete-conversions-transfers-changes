using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Services.TrustCache
{
    public class TrustCacheService(ITrustsV4Client trustsClient) : ITrustCache
    {
        private Dictionary<Ukprn, TrustDto> UkprnCache = new();
        private Dictionary<string, TrustDto> TrnCache = new();

        public async Task<TrustDto> GetTrustAsync(Ukprn ukprn)
        {
            if (UkprnCache.ContainsKey(ukprn))
            {
                return UkprnCache[ukprn];
            }

            var trust = await trustsClient.GetTrustByUkprn2Async(ukprn.ToString());

            AddTrustToCache(trust);

            return trust!;
        }

        public async Task<TrustDto> GetTrustByTrnAsync(string trn)
        {
            if (TrnCache.ContainsKey(trn))
            {
                return TrnCache[trn];
            }

            var trust = await trustsClient.GetTrustByTrustReferenceNumberAsync(trn);

            AddTrustToCache(trust);

            return trust;
        }

        private void AddTrustToCache(TrustDto trust)
        {
            if (trust != null)
            {
                if (!UkprnCache.ContainsKey(trust.Ukprn))
                {
                    UkprnCache.Add(trust.Ukprn, trust);
                }

                UkprnCache[trust.Ukprn] = trust;

                if (!TrnCache.ContainsKey(trust.ReferenceNumber))
                {
                    TrnCache.Add(trust.ReferenceNumber, trust);
                }

                TrnCache[trust.ReferenceNumber] = trust;
            }
        }

        public async Task HydrateCache(IEnumerable<Ukprn> ukprns)
        {
            var trusts = await trustsClient.GetByUkprnsAllAsync(ukprns.Select(ukprn => ukprn.ToString()));

            ////var trusts = ukprns.Select(ukprn => trustsClient.GetTrustByUkprn2Async(ukprn.ToString())).Select(t => t.Result);

            foreach (var trust in trusts)
            {
                AddTrustToCache(trust);
            }
        }
    }
}
