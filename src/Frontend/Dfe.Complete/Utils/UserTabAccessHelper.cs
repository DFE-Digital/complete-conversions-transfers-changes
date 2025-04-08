using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;

namespace Dfe.Complete.Utils;

public static class UserTabAccessHelper
{

    // Parent tabs
    public const string YourProjectsTabName = "";
    public const string YourTeamProjectsTabName = "your-team-projects";
    public const string AllProjectsTabName = "";
    public const string GroupsTabName = "groups";
    public const string ServiceSupportProjectsTabName = "service-support";
    
    public const string AllProjectHandoverTabName = "handover";

    public static bool UserHasTabAccess(ProjectTeam userTeam, string tabName)
    {
        if (tabName == YourTeamProjectsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == AllProjectHandoverTabName) return userTeam.TeamIsRdo();
        if (tabName == GroupsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.ServiceSupport || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == ServiceSupportProjectsTabName) return userTeam == ProjectTeam.ServiceSupport;
        return true;
    }
}