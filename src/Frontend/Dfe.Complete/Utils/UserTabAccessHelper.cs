using System.Security.Claims;
using Dfe.Complete.Domain.Constants;
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
    public const string TeamProjects_HandedOver = "team-projects-handed-over";

    public static bool UserHasTabAccess(ClaimsPrincipal user, string tabName)
    {
        if (tabName == YourProjectsTabName) return user.HasRole(UserRolesConstants.RegionalDeliveryOfficer) || (user.HasRole(UserRolesConstants.RegionalCaseworkServices) && !user.HasRole(UserRolesConstants.ManageTeam));
        if (tabName == YourTeamProjectsTabName) return user.HasRole(UserRolesConstants.RegionalDeliveryOfficer) || user.HasRole(UserRolesConstants.RegionalCaseworkServices);
        if (tabName == AllProjects_HandoverTabName) return user.HasRole(UserRolesConstants.RegionalDeliveryOfficer) || user.HasRole(UserRolesConstants.ServiceSupport);
        if (tabName == GroupsTabName) return user.HasRole(UserRolesConstants.RegionalDeliveryOfficer) || user.HasRole(UserRolesConstants.ServiceSupport) || user.HasRole(UserRolesConstants.RegionalCaseworkServices);
        if (tabName == ServiceSupportProjectsTabName) return user.HasRole(UserRolesConstants.ServiceSupport);
        if (tabName == AllProjects_ExportsTabName) return user.HasRole(UserRolesConstants.ServiceSupport) || user.HasRole(UserRolesConstants.BusinessSupport) || user.HasRole(UserRolesConstants.DataConsumers)
            || (user.HasRole(UserRolesConstants.ManageTeam) && (user.HasRole(UserRolesConstants.RegionalDeliveryOfficer) || user.HasRole(UserRolesConstants.RegionalCaseworkServices)));
        if (tabName == TeamProjects_HandedOver) return user.HasRole(UserRolesConstants.RegionalDeliveryOfficer);
        return true;
    }
}