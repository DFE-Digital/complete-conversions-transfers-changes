using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByUser;

public class ProjectsByUser(ISender sender) : AllProjectsModel(ByUserNavigation)
{
    public UserWithProjectsDto UserResult { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var getUserQuery = new GetUserWithProjectsQuery(new UserId(new Guid(id)), ProjectState.Active)
            { Page = PageNumber - 1, Count = PageSize };

        var response = await sender.Send(getUserQuery);
        UserResult = response.Value ?? default!;
        
        Pagination = new PaginationModel($"/projects/all/users/{id}", PageNumber, response.ItemCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }
}