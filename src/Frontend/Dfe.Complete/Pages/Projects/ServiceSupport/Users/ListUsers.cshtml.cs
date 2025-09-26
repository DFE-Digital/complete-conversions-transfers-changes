
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.Users;

public class ListUsers(ISender sender) : ServiceSupportModel(UsersNavigation)
{
    public List<ListAllUsersWithProjectsResultModel> Users { get; set; } = default!;
    public async Task<IActionResult> OnGetAsync()
    {
        var usersResponse = await sender.Send(new ListAllUsersWithProjectsQuery(null, null, false) { Page = PageNumber - 1, Count = PageSize });
        Users = usersResponse.Value ?? [];

        Pagination = new PaginationModel(RouteConstants.ServiceSupportUsers, PageNumber, usersResponse.ItemCount, PageSize);
        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }
}
