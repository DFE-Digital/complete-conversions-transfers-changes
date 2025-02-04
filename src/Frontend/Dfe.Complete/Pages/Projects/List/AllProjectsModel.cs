using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List;

public class AllProjectsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }
    
    internal int PageSize = 20;
    
    public static string GetProjectSummaryUrl(ListAllProjectsResultModel project)
    {
        return string.Format(project.ProjectType == ProjectType.Conversion ? RouteConstants.ConversionProjectTaskList : RouteConstants.TransferProjectTaskList, project.ProjectId);
    }
    
    public static string GetProjectSummaryUrl(Project project)
    {
        return string.Format(project.Type == ProjectType.Conversion ? RouteConstants.ConversionProjectTaskList : RouteConstants.TransferProjectTaskList, project.Id);
    }
}