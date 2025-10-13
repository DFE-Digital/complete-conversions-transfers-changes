using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmIncomingTrustHasCompletedAllActions
{
    public class ConfirmIncomingTrustHasCompletedAllActionsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmIncomingTrustHasCompletedAllActionsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmIncomingTrustHasCompletedAllActions)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
