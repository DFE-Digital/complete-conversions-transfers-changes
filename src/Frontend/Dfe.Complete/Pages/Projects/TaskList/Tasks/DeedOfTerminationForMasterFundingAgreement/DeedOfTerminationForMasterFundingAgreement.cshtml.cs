using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfTerminationForMasterFundingAgreement
{
    public class DeedOfTerminationForMasterFundingAgreementModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeedOfTerminationForMasterFundingAgreementModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfTerminationForMasterFundingAgreement)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
