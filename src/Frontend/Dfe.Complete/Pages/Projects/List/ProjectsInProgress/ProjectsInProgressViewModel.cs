using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List;

public class ProjectsInProgressViewModel : PageModel
{
    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel Pagination { get; set; } = default!;
    
    internal int PageSize = 20;

    public static string GetProjectSummaryUrl(ListAllProjectsResultModel project)
    {
        return string.Format(project.ProjectType == ProjectType.Conversion ? RouteConstants.ConversionProjectTaskList : RouteConstants.TransferProjectTaskList, project.ProjectId);
    }
}