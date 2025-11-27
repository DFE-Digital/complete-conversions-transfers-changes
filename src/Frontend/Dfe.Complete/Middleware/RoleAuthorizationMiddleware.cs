using Dfe.Complete.Configuration;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Middleware;

public class RoleAuthorizationMiddleware(
    RequestDelegate next,
    ILogger<RoleAuthorizationMiddleware> logger,
    IOptions<AzureAdOptions> azureAdOptions)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RoleAuthorizationMiddleware> _logger = logger;
    private readonly AzureAdOptions _azureAdOptions = azureAdOptions.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (HasAllowAnonymousAttribute(context))
        {
            await _next(context);
            return;
        }

        // Only check roles for authenticated users
        if (context.User.Identity?.IsAuthenticated == true && !HasAllowedRole(context.User))
        {
            // Redirect to access denied page
            context.Response.Redirect("/access-denied");
            return;
        }

        await _next(context);
    }

    private static bool HasAllowAnonymousAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
            return false;

        // Check if the endpoint has AllowAnonymous attribute
        return endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;
    }

    private bool HasAllowedRole(System.Security.Claims.ClaimsPrincipal user)
    {
        if (_azureAdOptions.AllowedRoles == null || _azureAdOptions.AllowedRoles.Count == 0)
        {
            _logger.LogWarning("No allowed roles configured. Allowing all authenticated users.");
            return true;
        }

        return _azureAdOptions.AllowedRoles.Any(user.IsInRole);
    }
}