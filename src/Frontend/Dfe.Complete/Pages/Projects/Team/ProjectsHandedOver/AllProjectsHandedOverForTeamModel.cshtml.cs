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

namespace Dfe.Complete.Pages.Projects.Team.ProjectsHandedOver;

[Authorize(Policy = "CanViewTeamProjectsHandedOver")]
public class AllProjectsHandedOverForTeamModel(ISender sender) : YourTeamProjectsModel(HandedOverNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = [];

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);
        var recordCount = 0;

        if (userTeam.TeamIsRegionalDeliveryOfficer())
        {
            var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            var listProjectsForRegionQuery =
                new ListAllProjectsForTeamHandoverQuery((Region)userRegion!, ProjectState.Active, null)
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };

            var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);
            recordCount = listProjectsForRegionResult.ItemCount;
            Projects = listProjectsForRegionResult.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsHandedOver, PageNumber, recordCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}