using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfTerminationForChurchSupplementalAgreement
{
    public class DeedOfTerminationForChurchSupplementalAgreementModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeedOfTerminationForChurchSupplementalAgreementModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfTerminationForChurchSupplementalAgreement)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
