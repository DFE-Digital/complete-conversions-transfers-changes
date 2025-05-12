using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks
{
    public class EditLandRegistryTaskModel : PageModel
    {
        [BindProperty(SupportsGet = true, Name = "projectId")]
        public Guid ProjectId { get; set; }

        public string SchoolName { get; set; }

        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }
      

        public async Task<IActionResult> OnPost()
        {
            return null; 
            // return Redirect(string.Format(RouteConstants.ConversionLandRegistryTask, ProjectId));
        }
    }
}
