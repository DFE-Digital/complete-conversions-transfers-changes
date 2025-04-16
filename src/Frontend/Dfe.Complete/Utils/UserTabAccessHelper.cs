using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;

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

    public static bool UserHasTabAccess(ProjectTeam userTeam, string tabName)
    {

        if (tabName == YourProjectsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == YourTeamProjectsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == AllProjects_HandoverTabName) return userTeam.TeamIsRdo();
        if (tabName == GroupsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.ServiceSupport || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == ServiceSupportProjectsTabName) return userTeam == ProjectTeam.ServiceSupport;
        if (tabName == AllProjects_ExportsTabName) return userTeam is ProjectTeam.ServiceSupport or ProjectTeam.BusinessSupport or ProjectTeam.DataConsumers;
        return true;
    }
}