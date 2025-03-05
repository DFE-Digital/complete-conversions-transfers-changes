using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public class CompleteApiClient : ApiClient
    {
        public CompleteApiClient(
            IHttpClientFactory clientFactory, 
            ILogger<ApiClient> logger,
            string httpClientName = "CompleteClient") : base(clientFactory, logger, httpClientName)
        {
            
        }
    }
}
