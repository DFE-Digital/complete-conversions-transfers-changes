using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress;

public class TransferProjectsInProgressModel(ISender sender) : ConversionOrTransferInProgressModel(TransfersSubNavigation, ProjectType.Transfer)
{
    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
        var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, ProjectType.Transfer, AssignedToState.AssignedOnly, Page: PageNumber - 1, Count: PageSize);

        var response = await sender.Send(listProjectQuery);
        Projects = response.Value?.ToList() ?? [];

        var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, ProjectType.Transfer, AssignedToState.AssignedOnly);
        var countResponse = await sender.Send(countProjectQuery);

        Pagination = new PaginationModel("/projects/all/in-progress/transfers", PageNumber, countResponse.Value, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }

    public async Task<IActionResult> OnGetMovePage()
    {
        return await OnGet();
    }
}
