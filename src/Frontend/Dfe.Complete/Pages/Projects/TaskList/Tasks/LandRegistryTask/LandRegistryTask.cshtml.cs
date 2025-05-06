using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandRegistryTask
{
    public class LandRegistryTaskModel(ISender sender) : BaseProjectPageModel(sender)
    {
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
