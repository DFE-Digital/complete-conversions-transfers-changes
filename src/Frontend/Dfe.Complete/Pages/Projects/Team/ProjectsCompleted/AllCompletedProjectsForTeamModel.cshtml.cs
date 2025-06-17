using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Domain.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Team.ProjectsCompleted;

public class AllCompletedProjectsForTeamModel(ISender sender) : YourTeamProjectsModel(CompletedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = [];
    public bool UserTeamIsRegionalDeliveryOfficer { get; set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);
        UserTeamIsRegionalDeliveryOfficer = userTeam.TeamIsRegionalDeliveryOfficer();

        int recordCount = 0;

        var orderBy = new OrderProjectQueryBy(OrderProjectByField.CompletedAt, OrderByDirection.Descending);

        if (UserTeamIsRegionalDeliveryOfficer)
        {
            var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForRegionQuery((Region)userRegion!, ProjectState.Completed, null, null, orderBy)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);
            recordCount = listProjectsForRegionResult.ItemCount;
            Projects = listProjectsForRegionResult.Value ?? [];
        }
        else if (userTeam.TeamIsRegionalCaseworkServices())
        {
            var listProjectsForTeamQuery =
                new ListAllProjectsForTeamQuery(userTeam, ProjectState.Completed, null, null, orderBy)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listResponse = await sender.Send(listProjectsForTeamQuery);
            recordCount = listResponse.ItemCount;
            Projects = listResponse.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsCompleted, PageNumber, recordCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
        return hasPageFound ?? Page();
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}