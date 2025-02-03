using Dfe.Complete.Application.Projects.Queries.ListAllProjectsByRegion;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class AllProjectsByRegion(ISender sender) : PageModel
{
    public async Task OnGet()
    {
        var listProjectsByRegionQuery = new ListAllProjectsByRegionQuery(ProjectState.Active, null);

        var listResponse = await sender.Send(listProjectsByRegionQuery);
    }
}