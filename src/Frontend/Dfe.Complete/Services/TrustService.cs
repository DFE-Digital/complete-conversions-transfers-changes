using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Gateways;

namespace Dfe.Complete.Services
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
