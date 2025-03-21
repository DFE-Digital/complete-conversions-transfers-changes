﻿using Microsoft.Net.Http.Headers;

namespace Dfe.Complete.Authorization
{
    public static class AutomationHandler
    {
        public static bool ClientSecretHeaderValid(IHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            //Header authorisation not applicable for production
            if (!hostEnvironment.IsStaging() && !hostEnvironment.IsDevelopment())
            {
                return false;
            }

            //Allow client secret in header
            var authHeader = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?
                .Replace("Bearer ", string.Empty);

            var secret = configuration.GetValue<string>("CypressTestSecret");

            if (string.IsNullOrWhiteSpace(secret))
            {
                return false;
            }

            return authHeader == secret;
        }
    }
}
