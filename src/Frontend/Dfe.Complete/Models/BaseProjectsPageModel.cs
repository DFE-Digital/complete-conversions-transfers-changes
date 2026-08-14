using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Models;

public abstract class BaseProjectsPageModel(string currentNavigation) : PaginatedPageModel(currentNavigation)
{
    public static string GetProjectSummaryUrl(ListAllProjectsResultModel project) =>
        GetProjectSummaryUrl(project.ProjectId);

    public static string GetProjectSummaryUrl(ProjectId projectId) =>
        string.Format(RouteConstants.ProjectTaskList, projectId.Value);

    protected IActionResult HasPageFound(bool condition, int totalPages) =>
        condition && totalPages > 0 ? StatusCode(404) : null!;
}