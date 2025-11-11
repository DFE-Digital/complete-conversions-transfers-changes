using Dfe.Complete.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Infrastructure.Security.Authorization;

public class ActiveUserAuthorizationHandler : AuthorizationHandler<ActiveUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveUserRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(CustomClaimTypeConstants.UserId);

        if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}
