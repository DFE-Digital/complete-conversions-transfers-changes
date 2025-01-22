using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public class TrustService : AcademiesApiClient, ITrustService
    {
        private const string _url = @"v4/trusts/bulk";

        public TrustService(IHttpClientFactory httpClientFactory, ILogger<TrustService> logger) : base(httpClientFactory, logger)
        {
        }

        public async Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn)
        {
            var result = await GetEnumerable<TrustDetailsDto>($"{_url}?ukPrn={ukprn}");

            return result;
        }

        public async Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(IEnumerable<string> ukprns)
        {
            var query = ukprns.Select(ukprn => $"ukprns={ukprn}").Aggregate((acc, next) => acc + "&" + next);
            var result = await GetEnumerable<TrustDetailsDto>($"{_url}?{query}");

            return result;
        }
    }
}
