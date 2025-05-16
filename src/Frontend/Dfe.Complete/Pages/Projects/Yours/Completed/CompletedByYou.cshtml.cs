using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Yours.Completed;

public class CompletedByYou(ISender sender) : YourProjectsModel(CompletedNavigation)
{
    public List<ListAllProjectsForUserQueryResultModel>? ProjectsForUser { get; set; }

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourProjectsTabNavigationModel;

        var userAdId = User.GetUserAdId();

        var result = await sender.Send(new ListAllProjectsForUserQuery(ProjectState.Completed, userAdId, ProjectUserFilter.AssignedTo, new OrderProjectQueryBy(OrderProjectByField.CompletedAt, OrderByDirection.Descending))
            { Count = PageSize, Page = PageNumber - 1 });

        ProjectsForUser = result.Value ?? [];

        Pagination = new PaginationModel(RouteConstants.YourProjectsCompleted, PageNumber, result.ItemCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}