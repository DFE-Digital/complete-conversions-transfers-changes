using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Conversion.Tasks.HandoverWithDeliveryOfficer
{
    public class ViewHandoverWithDeliveryOfficerTaskModel : PageModel
    {
        [BindProperty(SupportsGet = true, Name = "projectId")]
        public string ProjectId { get; set; }

        // public GetConversionProjectByTaskResponse Project { get; set; }
        
        public async Task OnGet()
        {
        }
    }
}
