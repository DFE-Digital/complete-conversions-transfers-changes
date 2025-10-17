using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandConsentLetterTask
{
    public class LandConsentLetterTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LandConsentLetterTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LandConsentLetter)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
