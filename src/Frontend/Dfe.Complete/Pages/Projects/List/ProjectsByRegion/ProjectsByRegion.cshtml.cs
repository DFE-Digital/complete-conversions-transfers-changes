using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.ListAllProjectsByRegion;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class ProjectsByRegion(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Region { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    
    public PaginationModel Pagination { get; set; } = default!;

    public const int PageSize = 20;

    public List<ListAllProjectsResultModel>? ProjectsForRegion { get; set; }
    
    public async Task OnGet()
    {
        var listProjectsByRegionQuery = new ListAllProjectsByRegionQuery(ProjectState.Active, null);
        
        var result = await sender.Send(listProjectsByRegionQuery);
        
        var parsedRegion = Region.FromDescriptionValue<Region>();
        
        var projectsForRegion = result.Value
            .FirstOrDefault(p => p.Region == parsedRegion)
            .Projects
            .ToList();
        
        ProjectsForRegion = projectsForRegion.Skip(PageNumber - 1 * PageSize).Take(projectsForRegion.Count).ToList();
        
        Pagination = new PaginationModel($"/projects/all/regions/{Region}", PageNumber, projectsForRegion.Count, PageSize);
    }
    
    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}