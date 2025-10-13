using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmOutgoingTrustCeoDetails
{
    public class ConfirmOutgoingTrustCeoDetailsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmOutgoingTrustCeoDetailsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmOutgoingTrustCeoDetails)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
