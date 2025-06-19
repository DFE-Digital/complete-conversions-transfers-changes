using DfE.CoreLibs.Security.Interfaces;

namespace Dfe.Complete.Validators
{
    public class HasHeaderKeyExistsInRequestValidator(IConfiguration configuration, ILogger<HasHeaderKeyExistsInRequestValidator> logger) : ICustomRequestChecker
    {
        public bool IsValidRequest(HttpContext httpContext)
        {
            var headerKey = configuration["RequestHeaderKey"];
            var headerValue = configuration["RequestHeaderValue"];
            logger.LogInformation("Getting Anti forgery header key: {HeaderKey} and value: {HeaderValue} from configuration", headerKey, headerValue);
            if (string.IsNullOrWhiteSpace(headerKey) || string.IsNullOrWhiteSpace(headerValue) || !httpContext.Request.Headers.TryGetValue(headerKey, out var requestHeader))
            {
                var allHeaders = string.Join("; ",
                        httpContext.Request.Headers.Select(h =>
                            $"{h.Key}: {string.Join(",", h.Value!)}"));

                                logger.LogInformation("Getting Anti forgery header: {Headers}", !httpContext.Request.Headers.TryGetValue(headerKey, out var requestHeaderK));
                return false;
            }
            logger.LogInformation("Getting Anti forgery Result: {HeaderKey}", string.Equals(requestHeader, headerValue, StringComparison.Ordinal));
            return string.Equals(requestHeader, headerValue, StringComparison.Ordinal);
        }
    }
}
