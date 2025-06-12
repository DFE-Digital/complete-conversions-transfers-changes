using DfE.CoreLibs.Security.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Dfe.Complete.Validators
{
    public class HasHeaderKeyExistsInRequestValidator : ICustomRequestChecker
    {
        public bool IsValidRequest(HttpContext httpContext, string? headerKey, string? headerValue)
        {
            if (string.IsNullOrWhiteSpace(headerKey) || string.IsNullOrWhiteSpace(headerValue) || !httpContext.Request.Headers.TryGetValue(headerKey, out var requestHeader))
            {
                return false;
            }
            return string.Equals(requestHeader, headerValue, StringComparison.Ordinal);
        }
    }
}
