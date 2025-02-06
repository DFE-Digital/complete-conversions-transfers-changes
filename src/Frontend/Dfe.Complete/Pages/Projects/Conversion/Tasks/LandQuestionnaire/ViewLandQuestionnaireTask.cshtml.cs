
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Conversion.Tasks.LandQuestionnaire
{
    public class ViewLandQuestionnaireTaskModel : PageModel
    {

        [BindProperty(SupportsGet = true, Name = "projectId")]
        public string ProjectId { get; set; }
    }
}
