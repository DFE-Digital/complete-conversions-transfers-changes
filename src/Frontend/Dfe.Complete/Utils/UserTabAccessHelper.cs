using System.Security.Claims;
using Dfe.Complete.Extensions;

namespace Dfe.Complete.Utils;

public static class UserTabAccessHelper
{

    // TODO this may become a more general policy/authorisation helper
    // Main navs
    public const string YourProjectsTabName = "your-projects";
    public const string YourTeamProjectsTabName = "your-team-projects";
    public const string AllProjectsTabName = "all-projects";
    public const string GroupsTabName = "groups";
    public const string ServiceSupportProjectsTabName = "service-support";

    // Page navs
    public const string AllProjects_HandoverTabName = "all-projects-handover";
    public const string AllProjects_ExportsTabName = "all-projects-exports";

    public static bool UserHasTabAccess(ClaimsPrincipal user, string tabName)
    {
        if (tabName == YourProjectsTabName) return user.HasRole("regional_delivery_officer") || (user.HasRole("regional_casework_services") && !user.HasRole("manage_team"));
        if (tabName == YourTeamProjectsTabName) return user.HasRole("regional_delivery_officer") || user.HasRole("regional_casework_services");
        if (tabName == AllProjects_HandoverTabName) return user.HasRole("regional_delivery_officer") || user.HasRole("service_support");
        if (tabName == GroupsTabName) return user.HasRole("regional_delivery_officer") || user.HasRole("service_support") || user.HasRole("regional_casework_services");
        if (tabName == ServiceSupportProjectsTabName) return user.HasRole("service_support");
        if (tabName == AllProjects_ExportsTabName) return user.HasRole("service_support") || user.HasRole("business_support") || user.HasRole("data_consumers")
            || (user.HasRole("manage_team") && (user.HasRole("regional_delivery_officer") || user.HasRole("regional_casework_services")));
        return true;
    }
}