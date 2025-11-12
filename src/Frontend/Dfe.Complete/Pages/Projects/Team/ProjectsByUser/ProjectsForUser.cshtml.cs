using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Team.ProjectsForUser;

public class ProjectsForUser(ISender sender) : YourTeamProjectsModel(ByUserNavigation)
{
    public UserWithProjectsDto UserResult { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var getUserQuery = new GetUserWithProjectsQuery(new UserId(new Guid(id)), ProjectState.Active, OrderUserProjectsByField.SignificantDate)
        { Page = PageNumber - 1, Count = PageSize };

        var response = await sender.Send(getUserQuery);
        UserResult = response.Value ?? default!;

        Pagination = new PaginationModel($"/projects/team/users/{id}", PageNumber, response.ItemCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }
}