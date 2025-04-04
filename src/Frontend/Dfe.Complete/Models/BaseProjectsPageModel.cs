using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models;

public abstract class BaseProjectsPageModel(string currentNavigation) : PageModel
{
    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;

    public static string GetProjectSummaryUrl(ListAllProjectsResultModel project) => 
        GetProjectSummaryUrl(project.ProjectType, project.ProjectId);

    public static string GetProjectSummaryUrl(ProjectType? type, ProjectId projectId)
    {
        return string.Format(
            type == ProjectType.Conversion
                ? RouteConstants.ConversionProjectTaskList
                : RouteConstants.TransferProjectTaskList,
            projectId.Value);
    }
}