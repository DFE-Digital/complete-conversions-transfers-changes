using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Team.ProjectsByUser;

public class AllProjectsByUser(ISender sender) : YourTeamProjectsModel(ByUserNavigation)
{
    public List<ListAllUsersWithProjectsResultModel> Users { get; set; } = [];

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);

        var usersQuery = new ListAllUsersWithProjectsQuery(ProjectState.Active, userTeam, false)
        {
            Page = PageNumber - 1,
            Count = PageSize
        };

        var result = await sender.Send(usersQuery);
        var recordCount = result.ItemCount;
        Users = result.Value ?? [];

        Pagination = new PaginationModel(RouteConstants.TeamProjectsUsers, PageNumber, recordCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }

    public async Task<IActionResult> OnGetMovePage()
    {
        return await OnGet();
    }
}