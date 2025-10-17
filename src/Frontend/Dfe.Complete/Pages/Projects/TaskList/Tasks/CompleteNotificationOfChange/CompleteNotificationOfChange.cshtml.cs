using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.CompleteNotificationOfChange
{
    public class CompleteNotificationOfChangeModel(ISender sender, IAuthorizationService authorizationService, ILogger<CompleteNotificationOfChangeModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CompleteNotificationOfChange)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    } 
}
