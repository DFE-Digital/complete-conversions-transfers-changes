using Dfe.Complete.Application.Projects.Models;
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

public class AllCompletedProjectsForTeamModel(ISender sender) : YourTeamProjectsModel(CompletedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
    public ProjectTeam UserTeam { get; set; } = new();

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;
        
        var userQuery = new GetUserByAdIdQuery(User.GetUserAdId());
        var userResponse = (await sender.Send(userQuery))?.Value;
        var userTeam = EnumExtensions.FromDescription<ProjectTeam>(userResponse?.Team);
        int recordCount = 0;

        if (EnumHelper.TeamIsGeographic(userTeam))
        {
            var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForRegionQuery((Region)userRegion, ProjectState.Completed, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);
            recordCount = listProjectsForRegionResult.ItemCount;
            Projects = listProjectsForRegionResult.Value ?? [];
        }
        else if (userTeam == ProjectTeam.RegionalCaseWorkerServices)
        {
            var listProjectsForTeamQuery =
                new ListAllProjectsForTeamQuery(userTeam, ProjectState.Completed, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listResponse = await sender.Send(listProjectsForTeamQuery);
            recordCount = listResponse.ItemCount;
            Projects = listResponse.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsCompleted, PageNumber, recordCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}