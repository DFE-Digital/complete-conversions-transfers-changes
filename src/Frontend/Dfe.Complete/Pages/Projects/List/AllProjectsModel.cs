﻿using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.List;

public abstract class AllProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel AllProjectsTabNavigationModel = new(TabNavigationModel.AllProjectsTabName);

    public const string HandoverNavigation = "all-projects-handover";
    public const string InProgressNavigation = "in-progress";
    public const string ByMonthNavigation = "by-month";
    public const string ByRegionNavigation = "by-region";
    public const string ByUserNavigation = "by-user";
    public const string ByTrustNavigation = "by-trust";
    public const string ByLocalAuthorityNavigation = "by-local-authority";
    public const string CompletedNavigation = "completed";
    public const string StatisticsNavigation = "Statistics";
    public const string ReportsNavigation = "all-projects-reports";

    public static string GetTrustProjectsUrl(ListTrustsWithProjectsResultModel trustModel)
    {
        return trustModel.Identifier.Contains("TR")
            ? string.Format(RouteConstants.TrustMATProjects, trustModel.Identifier)
            : string.Format(RouteConstants.TrustProjects, trustModel.Identifier);
    }
}