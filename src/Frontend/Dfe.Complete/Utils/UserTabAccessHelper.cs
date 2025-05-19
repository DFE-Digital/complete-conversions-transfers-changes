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
    public const string TeamProjects_Unassigned = "team-projects-unassigned";

    public static bool UserHasTabAccess(ClaimsPrincipal user, string tabName)
    {
        var isRdo = user.HasRole(UserRolesConstants.RegionalDeliveryOfficer);
        var isCase = user.HasRole(UserRolesConstants.RegionalCaseworkServices);
        var isService = user.HasRole(UserRolesConstants.ServiceSupport);
        var isBusiness = user.HasRole(UserRolesConstants.BusinessSupport);
        var isData = user.HasRole(UserRolesConstants.DataConsumers);
        var isManageTeam = user.HasRole(UserRolesConstants.ManageTeam);

        return tabName switch
        {
            YourProjectsTabName => isRdo || (isCase && !isManageTeam),
            YourTeamProjectsTabName => isRdo || isCase,
            AllProjects_HandoverTabName => isRdo || isService,
            GroupsTabName => isRdo || isService || isCase,
            ServiceSupportProjectsTabName => isService,
            AllProjects_ExportsTabName => isService || isBusiness || isData || (isManageTeam && (isRdo || isCase)),
            TeamProjects_HandedOver => isRdo,
            TeamProjects_Unassigned => isManageTeam && (isRdo || isCase),
            _ => true
        };
    }
}