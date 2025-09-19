using Dfe.Complete.Models.ProjectCompletion;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Completion
{
    public class _ConversionValidationNotificationModel : PageModel
    {
        public ConversionCompletionValidationResultModel? ConversionCompletionValidationResult { get; set; }

        public void OnGet()
        {
            Page();
        }
    }
}
