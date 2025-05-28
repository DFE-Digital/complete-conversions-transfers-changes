using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class ProjectsByRegion(ISender sender) : AllProjectsModel(ByRegionNavigation)
{
    [BindProperty(SupportsGet = true)] public string? Region { get; set; }

    public List<ListAllProjectsResultModel>? Projects { get; set; }
    public Region RegionName { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var parsedRegion = Region.FromDescriptionValue<Region>();

        if (parsedRegion == null) return NotFound();
        
        RegionName = (Region)parsedRegion!;

        var listProjectsForRegionQuery =
            new ListAllProjectsForRegionQuery(RegionName, ProjectState.Active, null)
            {
                Page = PageNumber - 1,
                Count = PageSize
            };

        var listProjectsForRegionResult = await sender.Send(listProjectsForRegionQuery);

        Projects = listProjectsForRegionResult.Value;

        Pagination = new PaginationModel($"/projects/all/regions/{Region}", PageNumber,
            listProjectsForRegionResult.ItemCount, PageSize);

        return Page();
    }

    public async Task OnGetMovePage()
    {
        await OnGetAsync();
    }
}