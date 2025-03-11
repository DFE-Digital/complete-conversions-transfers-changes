using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsForTeam;

public class AllProjectsInProgressForTeamModel(ISender sender) : YourTeamProjectsModel(InProgressNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userQuery = new GetUserByAdIdQuery(User.GetUserAdId());
        var userResponse = await sender.Send(userQuery);
        var userTeam = EnumExtensions.FromDescription<ProjectTeam>(userResponse?.Value?.Team);

        var listProjectsForTeamQuery =
            new ListAllProjectsForTeamQuery(userTeam, ProjectState.Active, null)
            {
                Page = PageNumber - 1,
                Count = PageSize
            };

        var listResponse = await sender.Send(listProjectsForTeamQuery);
        Projects = listResponse.Value ?? [];

        Pagination = new PaginationModel(RouteConstants.TeamProjectsInProgress, PageNumber, listResponse.ItemCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}