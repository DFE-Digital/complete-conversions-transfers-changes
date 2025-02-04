using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByUser;

public class ProjectsByUser(ISender sender) : AllProjectsModel
{
    public UserWithProjectsResultModel UserResult { get; set; } = default!;

    public async Task OnGet(string id)
    {
        var getUserQuery = new GetUserWithProjectsQuery(new UserId(new Guid(id)), ProjectState.Active)
            { Page = PageNumber - 1, Count = PageSize };

        var response = await sender.Send(getUserQuery);
        UserResult = response.Value ?? default!;
        
        Pagination = new PaginationModel($"/projects/all/users/{id}", PageNumber, response.ItemCount, PageSize);

    }
}