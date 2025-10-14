using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmChairOfGovernorsDetails
{
    public class ConfirmChairOfGovernorsDetailsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmChairOfGovernorsDetailsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmChairOfGovernorsDetails)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
