using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Conversion
{
    public class ProjectCreatedModel : PageModel
    {

        [BindProperty(SupportsGet = true, Name = "projectId")]
        public Guid ProjectId { get; set; }

        public IActionResult OnPost()
        {
            return Redirect($"/projects/{ProjectId}/tasks");
        }
    }
}
