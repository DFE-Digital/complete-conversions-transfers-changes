using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Gateways;

namespace Dfe.Complete.Services
{
    public class TrustService(IHttpClientFactory httpClientFactory, ILogger<TrustService> logger) : AcademiesApiClient(httpClientFactory, logger), ITrustService
    {
		private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
		private readonly ILogger<TrustService> _logger = logger;
		private const string Url = "v4/trusts";

		public async Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn)
		{
			var result = await GetEnumerable<TrustDetailsDto>($"{Url}?ukPrn={ukprn}");
            return result;
		}
	}
}
