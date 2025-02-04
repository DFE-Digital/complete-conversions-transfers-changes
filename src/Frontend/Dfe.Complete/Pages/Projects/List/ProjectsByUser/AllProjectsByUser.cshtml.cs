using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByUser;

public class AllProjectsByUser(ISender sender) : AllProjectsModel
{
    public List<UserWithProjectsResultModel> Users { get; set; } = default!;

    public async Task OnGet()
    {
        var listProjectQuery = new ListAllUsersWithProjectsQuery { Page = PageNumber - 1, Count = PageSize, State = ProjectState.Active};

        var listResponse = await sender.Send(listProjectQuery);
        Users = listResponse.Value ?? [];

        Pagination = new PaginationModel("/projects/all/users", PageNumber, listResponse.ItemCount, PageSize);
    }
}