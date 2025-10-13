using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmTransferGrantFundingLevel
{
    public class ConfirmTransferGrantFundingLevelModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmTransferGrantFundingLevelModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmTransferGrantFundingLevel)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
