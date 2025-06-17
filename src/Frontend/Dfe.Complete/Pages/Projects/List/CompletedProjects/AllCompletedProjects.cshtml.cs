using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.CompletedProjects;

public class AllCompletedProjectsViewModel(ISender sender) : AllProjectsModel(CompletedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var orderByClause = new OrderProjectQueryBy(OrderProjectByField.CompletedAt, OrderByDirection.Descending);
        var listProjectQuery = new ListAllProjectsQuery(ProjectState.Completed, null, OrderBy: orderByClause, Page: PageNumber - 1, Count: PageSize);

        var listResponse = await sender.Send(listProjectQuery);
        Projects = [.. (listResponse.Value ?? [])
            .Where(p => p.ProjectCompletionDate != null)
            .OrderByDescending(p => p.ProjectCompletionDate)];

        var countProjectQuery = new CountAllProjectsQuery(ProjectState.Completed, null);
        var countResponse = await sender.Send(countProjectQuery);

        Pagination = new PaginationModel(RouteConstants.CompletedProjects, PageNumber, countResponse.Value, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }

    public async Task<IActionResult> OnGetMovePage() => await OnGet();
}
