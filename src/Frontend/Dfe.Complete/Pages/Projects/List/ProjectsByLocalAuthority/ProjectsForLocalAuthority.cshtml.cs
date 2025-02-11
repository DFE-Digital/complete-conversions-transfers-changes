using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByLocalAuthority;

public class ProjectsForLocalAuthority : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string LocalAuthorityCode { get; set; }
    
    public string LocalAuthorityName { get; set; }
    
    public void OnGet()
    {
        
    }
}