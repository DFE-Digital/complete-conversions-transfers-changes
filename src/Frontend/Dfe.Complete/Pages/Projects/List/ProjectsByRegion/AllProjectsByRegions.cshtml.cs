using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class AllProjectsByRegions(ISender sender) : PageModel
{
    public List<ListAllProjectsByRegionsResultModel>? ListAllProjectsByRegionResultModel { get; set; }
    
    public async Task OnGet()
    {
        var listProjectsByRegionQuery = new ListAllProjectsByRegionQuery(ProjectState.Active, null);

        var result = await sender.Send(listProjectsByRegionQuery);
        
        ListAllProjectsByRegionResultModel = result.Value;
    }
}