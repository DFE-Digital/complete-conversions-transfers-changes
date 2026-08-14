namespace Dfe.Complete.Models;

public abstract class BaseSignificantChangeProjectsPageModel() : PaginatedPageModel(string.Empty)
{
    protected TabNavigationModel SignificantChangeTabNavigationModel = new(TabNavigationModel.SignificantChangeTabName);
}