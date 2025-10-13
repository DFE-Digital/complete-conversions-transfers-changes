using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmAndProcessSponsoredSupportGrant
{
    public class ConfirmAndProcessSponsoredSupportGrantModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmAndProcessSponsoredSupportGrantModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmAndProcessSponsoredSupportGrant)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
