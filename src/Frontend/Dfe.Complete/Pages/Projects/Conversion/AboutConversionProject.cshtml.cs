using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Conversion
{
    public class AboutConversionProjectModel : PageModel
    {

        [BindProperty(SupportsGet = true, Name = "projectId")]
        public string ProjectId { get; set; }
        
    }
}
