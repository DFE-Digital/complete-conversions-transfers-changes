using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.List;

public abstract class YourTeamProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel YourTeamProjectsTabNavigationModel = new(TabNavigationModel.YourTeamProjectsTabName);

    public const string InProgressNavigation = "in-progress";
}