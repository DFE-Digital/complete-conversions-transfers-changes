using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Domain.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Team.ProjectsUnassigned;

[Authorize(policy: UserPolicyConstants.CanViewTeamProjectsUnassigned)]
public class AllProjectsUnassignedForTeamModel(ISender sender) : YourTeamProjectsModel(UnassignedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = [];
    public bool UserTeamIsRegionalDeliveryOfficer { get; set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);
        UserTeamIsRegionalDeliveryOfficer = userTeam.TeamIsRegionalDeliveryOfficer();
        var recordCount = 0;

        if (UserTeamIsRegionalDeliveryOfficer)
        {
            var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForRegionQuery((Region)userRegion!, ProjectState.Active, null, AssignedToState.UnassignedOnly, null)
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
                new ListAllProjectsForTeamQuery(userTeam, ProjectState.Active, null, AssignedToState.UnassignedOnly, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listResponse = await sender.Send(listProjectsForTeamQuery);
            recordCount = listResponse.ItemCount;
            Projects = listResponse.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsUnassigned, PageNumber, recordCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage);
        return hasPageFound ?? Page();
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}