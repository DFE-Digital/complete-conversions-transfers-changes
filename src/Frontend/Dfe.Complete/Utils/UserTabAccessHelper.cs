using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;

namespace Dfe.Complete.Utils;

public static class UserTabAccessHelper
{
    public const string YourTeamProjectsTabName = "your-team-projects";
    public const string AllProjectHandoverTabName = "handover";

    public static bool UserHasTabAccess(ProjectTeam userTeam, string tabName)
    {
        if (tabName == YourTeamProjectsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        if (tabName == AllProjectHandoverTabName) return userTeam.TeamIsRdo();
        return true;
    }
}