using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.SupplementalFundingAgreementTask
{
    public class SupplementalFundingAgreementTaskModel(ISender sender) : BaseProjectPageModel(sender)
    {
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }

        [BindProperty(Name = "signed-secretary-state")]
        public bool? SignedSecretaryState { get; set; }

        
        public async Task<IActionResult> OnPost()
        {
            return Redirect(string.Format(RouteConstants.ProjectSupplementalFundingAgreementTask, ProjectId));
        }
    }
}
