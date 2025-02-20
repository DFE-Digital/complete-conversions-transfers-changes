using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects
{
    public class CreateNewProjectModel : PageModel
    {
        [BindProperty]
        public string? ProjectType { get; set; }
        
        public IActionResult OnPost()
        {
            var pageToRedirectTo = ProjectType switch
            {
                "conversion" => "/Projects/Conversion/CreateNewProject",
                "fam_conversion" => "/Projects/MatConversion/CreateNewProject",
                "transfer" => "/Projects/Transfer/CreateNewProject",
                "fam_transfer" => "/Projects/MatTransfer/CreateNewProject",
                _ => string.Empty
            };

            return RedirectToPage(pageToRedirectTo);
        }
    }
}
