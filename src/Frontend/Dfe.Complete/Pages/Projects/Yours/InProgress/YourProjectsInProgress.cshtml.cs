using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Yours.InProgress;

public class YourProjectsInProgress(ISender sender) : YourProjectsModel(InProgressNavigation)
{
    public List<ListAllProjectsForUserQueryResultModel>? ProjectsForUser { get; set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourProjectsTabNavigationModel;

        var userAdId = User.GetUserAdId();

        var result = await sender.Send(new ListAllProjectsForUserQuery(ProjectState.Active, userAdId,
                ProjectUserFilter.AssignedTo,
                new OrderProjectQueryBy
                    { Field = OrderProjectByField.SignificantDate, Direction = OrderByDirection.Descending })
            { Count = PageSize, Page = PageNumber - 1 });

        ProjectsForUser = result.Value ?? [];

        Pagination = new PaginationModel("/projects/yours/in-progress", PageNumber, result.ItemCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }

    public async Task OnGetMovePage() => await OnGet();
}