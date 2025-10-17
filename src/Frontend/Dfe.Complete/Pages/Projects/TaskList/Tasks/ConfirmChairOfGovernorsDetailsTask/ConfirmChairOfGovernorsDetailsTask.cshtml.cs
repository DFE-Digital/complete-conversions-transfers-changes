using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmChairOfGovernorsDetailsTask
{
    public class ConfirmChairOfGovernorsDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmChairOfGovernorsDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmChairOfGovernorsDetails)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
