using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByRegion;

public class ProjectsByRegion : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Region { get; set; }
    
    public void OnGet()
    {
        
    }
}