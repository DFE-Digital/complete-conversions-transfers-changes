using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public class AcademiesApiClient : ApiClient
    {
        private readonly ILogger<AcademiesApiClient> _logger;

        public AcademiesApiClient(
            IHttpClientFactory clientFactory, 
            ILogger<ApiClient> logger,
            string httpClientName = "AcademiesApiClient") : base(clientFactory, logger, httpClientName)
        {
            

        }

        public async Task<IEnumerable<T>> GetEnumerable<T>(string endpoint) where T : class
        {
            try
            {
                var result = await Get<ApiListWrapper<T>>(endpoint);

                if (result is { Data: { } })
                {
                    return result.Data;
                }

                return Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
