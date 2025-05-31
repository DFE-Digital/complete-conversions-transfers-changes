using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
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
        GetProjectSummaryUrl(project.ProjectId);

    public static string GetProjectSummaryUrl(ProjectId projectId) =>
        string.Format(RouteConstants.ProjectTaskList, projectId.Value);
}