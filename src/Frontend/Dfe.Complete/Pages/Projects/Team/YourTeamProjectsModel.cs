using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.Team;

[ExcludeFromCodeCoverage]
public abstract class YourTeamProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel YourTeamProjectsTabNavigationModel = new(TabNavigationModel.YourTeamProjectsTabName);

    public const string InProgressNavigation = "in-progress";
    public const string CompletedNavigation = "completed";
}