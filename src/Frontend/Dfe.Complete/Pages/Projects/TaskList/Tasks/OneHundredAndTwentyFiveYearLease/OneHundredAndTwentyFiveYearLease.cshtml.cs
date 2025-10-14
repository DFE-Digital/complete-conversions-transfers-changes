using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.OneHundredAndTwentyFiveYearLease
{
    public class OneHundredAndTwentyFiveYearLeaseModel(ISender sender, IAuthorizationService authorizationService, ILogger<OneHundredAndTwentyFiveYearLeaseModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.OneHundredAndTwentyFiveYearLease)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
