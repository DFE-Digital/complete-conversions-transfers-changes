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
                return false;
            }
            logger.LogInformation("Getting Anti forgery Result: {HeaderKey}", string.Equals(requestHeader, headerValue, StringComparison.Ordinal));
            return string.Equals(requestHeader, headerValue, StringComparison.Ordinal);
        }
    }
}
