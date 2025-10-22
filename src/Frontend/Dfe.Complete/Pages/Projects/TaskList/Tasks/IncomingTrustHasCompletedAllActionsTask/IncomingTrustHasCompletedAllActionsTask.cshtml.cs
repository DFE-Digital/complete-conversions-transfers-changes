using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.IncomingTrustHasCompletedAllActionsTask
{
    public class IncomingTrustHasCompletedAllActionsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<IncomingTrustHasCompletedAllActionsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmIncomingTrustHasCompletedAllActions)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
