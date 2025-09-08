using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Security;

public static class CustomPolicies
{
    public static Dictionary<string, Action<AuthorizationPolicyBuilder>> PolicyCustomizations => new()
    {
        [UserPolicyConstants.CanCreateProjects] = builder =>
        {
            builder.RequireAuthenticatedUser();
            builder.RequireAssertion(context =>
            {
                var user = context.User;
                return
                    user.IsInRole(UserRolesConstants.RegionalDeliveryOfficer) ||
                    (user.IsInRole(UserRolesConstants.RegionalCaseworkServices) && !user.IsInRole(UserRolesConstants.ManageTeam));
            });
        },
        [UserPolicyConstants.CanViewTeamProjectsUnassigned] = builder =>
        {
            builder.RequireAuthenticatedUser();
            builder.RequireAssertion(context =>
            {
                var user = context.User;
                return
                    user.IsInRole(UserRolesConstants.ManageTeam) &&
                    (user.IsInRole(UserRolesConstants.RegionalCaseworkServices) || user.IsInRole(UserRolesConstants.RegionalDeliveryOfficer));
            });
        },
        [UserPolicyConstants.ActiveUser] = builder =>
        {
            builder.RequireAuthenticatedUser();
            builder.AddRequirements(new ActiveUserRequirement());
        }
    };
}