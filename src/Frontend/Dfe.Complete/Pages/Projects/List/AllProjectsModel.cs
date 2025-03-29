using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.List;

public abstract class AllProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel AllProjectsTabNavigationModel = new(TabNavigationModel.AllProjectsTabName);

    public const string HandoverNavigation = "handover";
    public const string InProgressNavigation = "in-progress";
    public const string ByMonthNavigation = "by-month";
    public const string ByRegionNavigation = "by-region";
    public const string ByUserNavigation = "by-user";
    public const string ByTrustNavigation = "by-trust";
    public const string ByLocalAuthorityNavigation = "by-local-authority";
    public const string CompletedNavigation = "Completed";
    public const string StatisticsNavigation = "Statistics";

    public static string GetTrustProjectsUrl(ListTrustsWithProjectsResultModel trustModel)
    {
        return trustModel.identifier.Contains("TR") ? 
                                        string.Format(RouteConstants.TrustMATProjects, trustModel.identifier) 
                                        : string.Format(RouteConstants.TrustProjects, trustModel.identifier);
    }
}