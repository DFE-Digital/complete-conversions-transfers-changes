using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Pages.Projects.Team;

[ExcludeFromCodeCoverage]
[Authorize(Policy = "CanViewTeamProjects")]
public abstract class YourTeamProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel YourTeamProjectsTabNavigationModel = new(TabNavigationModel.YourTeamProjectsTabName);

    public const string UnassignedNavigation = "team-projects-unassigned";
    public const string InProgressNavigation = "in-progress";
    public const string CompletedNavigation = "completed";
    public const string NewNavigation = "new";
    public const string HandedOverNavigation = "team-projects-handed-over";
    public const string ByUserNavigation = "by-user";
}