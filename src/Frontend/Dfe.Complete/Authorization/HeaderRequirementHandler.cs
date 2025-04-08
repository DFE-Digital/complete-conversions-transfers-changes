using System.Security.Claims;
using Dfe.Complete.UserContext;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Authorization
{
    public class HeaderRequirementHandler : AuthorizationHandler<IAuthorizationRequirement>,
        IAuthorizationRequirement
    {
        private readonly IHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public HeaderRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IAuthorizationRequirement requirement)
        {
            if (AutomationHandler.ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
            {
                var simpleHeaders = _httpContextAccessor.HttpContext.Request.Headers
                    .Select(X => new KeyValuePair<string, string>(X.Key, X.Value.First()))
                    .ToArray();

                var userInfo = UserInfo.FromHeaders(simpleHeaders);

                var currentUser = context.User.Identities.FirstOrDefault();

                currentUser?.AddClaim(new Claim(ClaimTypes.Name, userInfo.Name));

                foreach (var claim in userInfo.Roles)
                {
                    currentUser?.AddClaim(new Claim(ClaimTypes.Role, claim));
                }

                currentUser?.AddClaim(new Claim(ClaimTypes.Authentication, "true"));

                if(currentUser?.Claims.All(c => c.Type != "objectidentifier") ?? true)
                {
                    if (!string.IsNullOrEmpty(userInfo.ActiveDirectoryId))
                    {
                        currentUser?.AddClaim(new Claim("objectidentifier", userInfo.ActiveDirectoryId));
                    }
                    else
                    {
                        currentUser?.AddClaim(new Claim("objectidentifier", "TEST-AD-ID"));
                    }
                }
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
} 