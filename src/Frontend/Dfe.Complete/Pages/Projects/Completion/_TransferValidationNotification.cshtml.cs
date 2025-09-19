using Dfe.Complete.Models.ProjectCompletion;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Completion
{
    public class _TransferValidationNotificationModel : PageModel
    {   
        public TransferCompletionValidationResultModel? TransferCompletionValidationResult { get; set; }

        public void OnGet()
        {
            Page();
        }

        public bool ShowNotification =>
            TransferCompletionValidationResult!.DateConfirmedAndInThePast ||
            TransferCompletionValidationResult.AuthorityToProceedCompleteTaskCompleted ||
            TransferCompletionValidationResult.ExpendentureCertificateTaskCompleted ||
            TransferCompletionValidationResult.AcademyTransferDateTaskCompleted;
    }
}
