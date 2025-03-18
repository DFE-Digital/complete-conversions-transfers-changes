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

public class AllProjectsInProgressForTeamModel(ISender sender) : YourTeamProjectsModel(InProgressNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
    public ProjectTeam UserTeam { get; set; } = new();

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userQuery = new GetUserByAdIdQuery(User.GetUserAdId());
        var userResponse = (await sender.Send(userQuery))?.Value;
        var UserTeam = EnumExtensions.FromDescription<ProjectTeam>(userResponse?.Team);

        if (EnumHelper.TeamIsGeographic(UserTeam))
        {
            var userRegion = EnumMapper.MapTeamToRegion(UserTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForRegionQuery((Region)userRegion, ProjectState.Active, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);
            Projects = listProjectsForRegionResult.Value;
        }
        else if (UserTeam == ProjectTeam.RegionalCaseWorkerServices)
        {
            var listProjectsForTeamQuery =
                new ListAllProjectsForTeamQuery(UserTeam, ProjectState.Active, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listResponse = await sender.Send(listProjectsForTeamQuery);
            Projects = listResponse.Value ?? [];
        }

        Projects = Projects?.Where(proj => !string.IsNullOrEmpty(proj.AssignedToFullName)).OrderBy(project => project.ConversionOrTransferDate).ToList();

        Pagination = new PaginationModel(RouteConstants.TeamProjectsInProgress, PageNumber, Projects.Count, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}