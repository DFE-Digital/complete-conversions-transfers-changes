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
                logger1.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
