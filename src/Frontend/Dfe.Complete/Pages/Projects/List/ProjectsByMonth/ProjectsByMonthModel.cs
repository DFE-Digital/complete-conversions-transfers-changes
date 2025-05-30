using System.Globalization;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByMonth;

public abstract class ProjectsByMonthModel(string currentSubNavigationItem) : AllProjectsModel(ByMonthNavigation)
{
    public const string ConversionsSubNavigation = "conversions";
    public const string TransfersSubNavigation = "transfers";
    public string CurrentSubNavigationItem { get; set; } = currentSubNavigationItem;
    
    [BindProperty(SupportsGet = true, Name = "fromMonth")]
    public int FromMonth { get; set; }
        
    [BindProperty(SupportsGet = true, Name = "fromYear")]
    public int FromYear { get; set; }
    
    [BindProperty(Name = "fromDate")]
    public string FromDate { get; set; } = GetDefaultDate();
        
    [BindProperty(Name = "toDate")]
    public string ToDate { get; set; } = GetDefaultDate();
    
    [BindProperty(SupportsGet = true, Name = "toMonth")]
    public int ToMonth { get; set; }
        
    [BindProperty(SupportsGet = true, Name = "toYear")]
    public int ToYear { get; set; }
    
    public string DateRangeDisplay { get; set; } = string.Empty;
    
    
    public List<ListProjectsByMonthResultModel> Projects { get; set; } = [];

    
    public static List<DateOnly?> GetMonths()
    {
        var currentYear = DateTime.Now.Year;
        var months = new List<DateOnly?>();

        for (int year = currentYear - 2; year <= currentYear + 2; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                months.Add(new DateOnly(year, month, 1));
            }
        }

        return months;
    }
    
    public static string GetDefaultDate()
    {
        var today = (DateOnly?)DateOnly.FromDateTime(DateTime.Now);
        return today.ToMonthYearString();
    }
    
    public bool TryParseInputDates(out DateTime fromDate, out DateTime toDate)
    {
        fromDate = default;
        toDate = default;
            
        bool fromSuccess = DateTime.TryParse(FromDate, out fromDate);
        bool toSuccess = DateTime.TryParse(ToDate, out toDate);
            
        return fromSuccess && toSuccess;
    }
    
    public static DateOnly? ParseDate(int month, int year)
    {
        if (DateOnly.TryParse($"{month}/1/{year}", CultureInfo.InvariantCulture, out var date))
        {
            return date;
        }
        return null;
    }
    
    public void SetErrorNotification(string message)
    {
        TempDataExtensions.SetNotification(
            TempData, 
            NotificationType.Error, 
            "Important", 
            message);
    }
    
    public static IActionResult RedirectToDateRange(string pathToPage, DateTime fromDate, DateTime toDate)
    {
        var url = string.Format(pathToPage, fromDate.Month, fromDate.Year, toDate.Month, toDate.Year);
        return new RedirectResult(url);
    }
    
    public static string GetProjectByMonthsUrl(ProjectType projectType, UserDto user, int fromMonth, int fromYear, int? toMonth, int? toYear)
    {
        var isConversion = projectType == ProjectType.Conversion;
        var canViewDataConsumerView = CanViewDataConsumerView(user);

        string path;
        if (canViewDataConsumerView)
        {
            path = isConversion
                ? RouteConstants.ConversionProjectsByMonths
                : RouteConstants.TransfersProjectsByMonths;
        }
        else
        {
            path = isConversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth;
        }

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