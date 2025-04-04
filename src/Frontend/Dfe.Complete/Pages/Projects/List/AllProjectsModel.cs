using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List;

public abstract class AllProjectsModel(string currentNavigation) : PageModel
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

    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;

    public static string GetProjectSummaryUrl(ListAllProjectsResultModel project)
    {
        return string.Format(
            project.ProjectType == ProjectType.Conversion
                ? RouteConstants.ConversionProjectTaskList
                : RouteConstants.TransferProjectTaskList, project.ProjectId.Value);
    }

    public static string GetProjectSummaryUrl(ProjectType? type, ProjectId projectId)
    {
        return string.Format(
            type == ProjectType.Conversion
                ? RouteConstants.ConversionProjectTaskList
                : RouteConstants.TransferProjectTaskList, projectId.Value);
    }
    
    public static string GetTrustProjectsUrl(ListTrustsWithProjectsResultModel trustModel)
    {
        return trustModel.identifier.Contains("TR") ? 
                                        string.Format(RouteConstants.TrustMATProjects, trustModel.identifier) 
                                        : string.Format(RouteConstants.TrustProjects, trustModel.identifier);
    }
    
    public static string GetProjectSummaryUrl(ProjectType projectType, string projectId)
    {
        return string.Format(projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectTaskList : RouteConstants.TransferProjectTaskList, projectId);
    }
    
    public static string GetProjectByMonthsUrl(ProjectType projectType, UserDto user)
    {
        DateTime date = DateTime.Now;
        string month = date.Month.ToString("0");
        string year = date.Year.ToString("0000");
        
        bool isConversion = projectType == ProjectType.Conversion;

        var singleMonthPath =
            isConversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth;
        
        var multipleMonthsPath =
            isConversion ? RouteConstants.ConversionProjectsByMonths : RouteConstants.TransfersProjectsByMonths;
        
        var standardView = string.Format(singleMonthPath, month, year);
        var dataConsumerUrl = string.Format(multipleMonthsPath, month, year, month, year);

        var canViewDataConsumerView = CanViewDataConsumerView(user);

        return canViewDataConsumerView ? dataConsumerUrl : standardView;
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