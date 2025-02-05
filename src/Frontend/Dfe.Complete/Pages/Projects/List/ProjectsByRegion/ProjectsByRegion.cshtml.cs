using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class ProjectsByRegion(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Region { get; set; }

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel Pagination { get; set; } = default!;

    public int PageSize = 20;

    public List<ListAllProjectsResultModel>? Projects { get; set; }

    public async Task OnGet()
    {
        var parsedRegion = Region.FromDescriptionValue<Region>();

        var listProjectsForRegion =
            new ListAllProjectsForRegionQuery(parsedRegion, ProjectState.Active, null, PageNumber - 1, PageSize);
        var listProjectsForRegionResult = await sender.Send(listProjectsForRegion);
        
        var listProjectsByRegionQuery = new ListAllProjectsByRegionQuery(ProjectState.Active, null);
        var listProjectsByRegionQueryResult = await sender.Send(listProjectsByRegionQuery);
        var projectForRegion = listProjectsByRegionQueryResult.Value
            .SingleOrDefault(x => x.Region == parsedRegion);

        var projectCountForRegion = projectForRegion.ConversionsCount + projectForRegion.TransfersCount; 
        
        Projects = listProjectsForRegionResult.Value;
        
        Pagination = new PaginationModel($"/projects/all/regions/{Region}", PageNumber, projectCountForRegion, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}