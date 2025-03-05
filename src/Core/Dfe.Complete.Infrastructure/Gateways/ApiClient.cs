using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace Dfe.Complete.Infrastructure.Gateways
{
    public abstract class ApiClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _httpClientName;

        protected ApiClient(
            IHttpClientFactory clientFactory, 
            ILogger<ApiClient> logger,
            string httpClientName)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _httpClientName = httpClientName;
        }

        public async Task<T> Get<T>(string endpoint) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                var client = CreateHttpClient();

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                // Read response content
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize content
                var result = JsonConvert.DeserializeObject<T>(content);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }


        public async Task<Stream> GetStream(string endpoint)
        {
            try 
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                var client = CreateHttpClient();

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var fileStream = await response.Content.ReadAsStreamAsync();

                return fileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TResult> Post<T, TResult>(string endpoint, T dto)
        {
            try
            {
                var request = new StringContent(
                    JsonConvert.SerializeObject(dto),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                var client = CreateHttpClient();

                var response = await client.PostAsync(endpoint, request);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(content);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TResult> Patch<T, TResult>(string endpoint, T dto)
        {
            try
            {
                var request = new StringContent(
                    JsonConvert.SerializeObject(dto),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                var client = CreateHttpClient();

                var response = await client.PatchAsync(endpoint, request);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(content);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TResult> Put<T, TResult>(string endpoint, T dto)
        {
            try
            {
                var request = new StringContent(
                    JsonConvert.SerializeObject(dto),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                var client = CreateHttpClient();

                var response = await client.PutAsync(endpoint, request);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(content);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = _clientFactory.CreateClient(_httpClientName);
            
            //TODO: we still using this class?
            // client.DefaultRequestHeaders.Add(HttpHeaderConstants.UserContextName, _httpContextAccessor.HttpContext.User?.Identity?.Name);

            return client;
        }
    }
}
