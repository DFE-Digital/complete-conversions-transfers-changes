using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmHeadTeacherDetails
{
    public class ConfirmHeadTeacherDetailsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmHeadTeacherDetailsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmHeadTeacherDetails)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
