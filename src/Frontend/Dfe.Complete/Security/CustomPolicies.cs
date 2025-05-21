using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Security;

public static class CustomPolicies
{
    public static Dictionary<string, Action<AuthorizationPolicyBuilder>> PolicyCustomizations => new Dictionary<string, Action<AuthorizationPolicyBuilder>>
    {
        [UserPolicies.CanCreateProjects.ToString()] = builder =>
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
        [UserPolicies.CanViewYourProjects.ToString()] = builder =>
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
        [UserPolicies.CanViewTeamProjectsUnassigned.ToString()] = builder =>
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
    };
}