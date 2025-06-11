using DfE.CoreLibs.Security.Interfaces;

namespace Dfe.Complete.Validators
{
    public class HasHeaderKeyExistsInRequestValidator : ICustomRequestChecker
    {
        public bool IsValidRequest(HttpContext httpContext, string? headerKey)
        {
            if (string.IsNullOrWhiteSpace(headerKey))
            {
                return false;
            }

            var requestHeader = httpContext.Request.Headers[headerKey];
            return !string.IsNullOrWhiteSpace(requestHeader);
        }
    }
}
