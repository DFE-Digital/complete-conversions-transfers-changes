using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.OneHundredAndTwentyFiveYearLeaseTask
{
    public class OneHundredAndTwentyFiveYearLeaseTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<OneHundredAndTwentyFiveYearLeaseTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.OneHundredAndTwentyFiveYearLease)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
