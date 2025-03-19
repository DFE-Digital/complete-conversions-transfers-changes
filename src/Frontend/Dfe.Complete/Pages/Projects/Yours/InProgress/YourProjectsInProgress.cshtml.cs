using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Yours.InProgress;

public class YourProjectsInProgress(ISender sender) : YourProjectsModel(InProgressNavigation)
{
    public List<ListAllProjectsForUserQueryResultModel>? ProjectsForUser { get; set; }

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourProjectsTabNavigationModel;

        var userAdId = User.GetUserAdId();

        var result = await sender.Send(new ListAllProjectForUserQuery(ProjectState.Active, userAdId)
            { Count = PageSize, Page = PageNumber - 1 });

        ProjectsForUser = result.Value ?? [];

        Pagination = new PaginationModel("/projects/yours/in-progress", PageNumber, result.ItemCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}