using GovUK.Dfe.CoreLibs.Security.Interfaces;

namespace Dfe.Complete.Validators
{
    public class HasHeaderKeyExistsInRequestValidator(IConfiguration configuration) : ICustomRequestChecker
    {
        public bool IsValidRequest(HttpContext httpContext)
        {
            var headerKey = configuration["RequestHeaderKey"];
            var headerValue = configuration["RequestHeaderValue"];
            if (string.IsNullOrWhiteSpace(headerKey) || string.IsNullOrWhiteSpace(headerValue) || !httpContext.Request.Headers.TryGetValue(headerKey, out var requestHeader))
            {
                return false;
            }
            return string.Equals(requestHeader, headerValue, StringComparison.Ordinal);
        }
    }
}
