using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Gateways;

namespace Dfe.Complete.Services
{
    public class TrustService : TramsApiClient, ITrustService
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<TrustService> _logger;
		private const string _url = @"v4/trusts";

		public TrustService(IHttpClientFactory httpClientFactory, ILogger<TrustService> logger) : base(httpClientFactory, logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn)
		{
			var result = await GetEnumerable<TrustDetailsDto>($"{_url}?ukPrn={ukprn}");

            return result;
		}
	}
}
