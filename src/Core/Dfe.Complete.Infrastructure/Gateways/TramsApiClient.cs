using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public class TramsApiClient : ApiClient
    {
        private readonly ILogger<TramsApiClient> _logger;

        public TramsApiClient(
            IHttpClientFactory clientFactory, 
            ILogger<ApiClient> logger,
            string httpClientName = "TramsApiClient") : base(clientFactory, logger, httpClientName)
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
