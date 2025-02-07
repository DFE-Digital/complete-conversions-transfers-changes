using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByUser;

public class AllProjectsByUser(ISender sender) : AllProjectsModel(ByUserNavigation)
{
    public List<UserWithProjectsDto> Users { get; set; } = default!;

    public new async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var listProjectQuery = new ListAllUsersWithProjectsQuery(ProjectState.Active) { Page = PageNumber - 1, Count = PageSize };

        var listResponse = await sender.Send(listProjectQuery);
        Users = listResponse.Value ?? [];

        Pagination = new PaginationModel("/projects/all/users", PageNumber, listResponse.ItemCount, PageSize);
    }
}