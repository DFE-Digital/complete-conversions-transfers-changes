using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Domain.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Pages.Projects.Team.HandedOver;

public class AllProjectsUnassignedForTeamModel(ISender sender) : YourTeamProjectsModel(UnassignedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = [];

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourTeamProjectsTabNavigationModel;

        var userTeam = await User.GetUserTeam(sender);
        var recordCount = 0;

        if (User.HasRole(UserRolesConstants.ManageTeam))
        {
            // var userRegion = EnumMapper.MapTeamToRegion(userTeam);

            // var listProjectsForRegionQuery =
            //     new ListAllProjectsForTeamHandoverQuery((Region)userRegion!, null, null)
            //     {
            //         Page = PageNumber - 1,
            //         Count = PageSize
            //     };

            // var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);
            // recordCount = listProjectsForRegionResult.ItemCount;
            // Projects = listProjectsForRegionResult.Value ?? [];
        }

        Pagination = new PaginationModel(RouteConstants.TeamProjectsUnassigned, PageNumber, recordCount, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}