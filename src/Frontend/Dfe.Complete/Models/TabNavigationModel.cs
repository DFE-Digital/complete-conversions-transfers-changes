﻿using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;

namespace Dfe.Complete.Models;

public class TabNavigationModel(string currentTab)
{
    public const string ViewDataKey = "TabNavigationModel";
    public const string YourProjectsTabName = "your-projects";
    public const string YourTeamProjectsTabName = "your-team-projects";
    public const string AllProjectsTabName = "all-projects";

    public string CurrentTab { get; } = currentTab;

    public static bool UserHasTabAccess(ProjectTeam userTeam, string tabName)
    {
        if (tabName == YourTeamProjectsTabName) return userTeam.TeamIsRdo() || userTeam == ProjectTeam.RegionalCaseWorkerServices;
        return true;
    }
}