using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Team.ProjectsNew;

public class AllNewProjectsForTeamModel(ISender sender) : YourTeamProjectsModel(NewNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
    public bool UserTeamIsRdo { get; set; }

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);
        UserTeamIsRdo = userTeam.TeamIsRdo();

        int recordCount = 0;

        if (UserTeamIsRdo)
        {
            var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForRegionQuery((Region)userRegion!, ProjectState.Active, null)
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
                new ListAllProjectsForTeamQuery(userTeam, ProjectState.Active, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listResponse = await sender.Send(listProjectsForTeamQuery);
            recordCount = listResponse.ItemCount;
            Projects = listResponse.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsNew, PageNumber, recordCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}