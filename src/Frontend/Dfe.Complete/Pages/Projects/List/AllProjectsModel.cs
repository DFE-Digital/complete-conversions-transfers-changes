using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
    public const string ExportsNavigation = "all-projects-exports";

    public static string GetTrustProjectsUrl(ListTrustsWithProjectsResultModel trustModel)
    {
        return trustModel.identifier.Contains("TR") 
            ? string.Format(RouteConstants.TrustMATProjects, trustModel.identifier)
            : string.Format(RouteConstants.TrustProjects, trustModel.identifier);
    }
    
    public static string GetProjectByMonthsUrl(ProjectType projectType, UserDto user, int fromMonth, int fromYear, int? toMonth, int? toYear)
    {
        bool isConversion = projectType == ProjectType.Conversion;
        bool canViewDataConsumerView = CanViewDataConsumerView(user);

        string path = canViewDataConsumerView
            ? (isConversion ? RouteConstants.ConversionProjectsByMonths : RouteConstants.TransfersProjectsByMonths)
            : (isConversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth);

        return canViewDataConsumerView
            ? string.Format(path, fromMonth, fromYear, toMonth, toYear)
            : string.Format(path, fromMonth + 1, fromYear);
    }


    private static bool CanViewDataConsumerView(UserDto user)
    {
        var allowedTeams = new List<ProjectTeam>() { ProjectTeam.DataConsumers, ProjectTeam.BusinessSupport, ProjectTeam.ServiceSupport };

        var managesTeam = user.ManageTeam.Value;
        var projectTeam = user.Team.FromDescriptionValue<ProjectTeam>().Value;
        
        var isInTeam = allowedTeams.Contains(projectTeam);
        
        var result = isInTeam || managesTeam;
        
        return result;
    }
    
    public static string GetProjectByMonthUrl(ProjectType projectType)
    {
        DateTime date = DateTime.Now.AddMonths(1);
        string month = date.Month.ToString("0");
        string year = date.Year.ToString("0000");

        return string.Format(projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth, month, year);
    }
}