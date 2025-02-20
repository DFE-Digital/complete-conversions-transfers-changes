using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public class AcademiesApiClient(
        IHttpClientFactory clientFactory,
        ILogger<ApiClient> logger,
        ILogger<AcademiesApiClient> logger1,
        string httpClientName = "AcademiesApiClient")
        : ApiClient(clientFactory, logger, httpClientName)
    {
    }
}
