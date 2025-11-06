using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmSchoolHasCompletedAllActions
{
    public class ConfirmSchoolHasCompletedAllActionsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmSchoolHasCompletedAllActionsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmSchoolHasCompletedAllActions)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            return Page();
        }
    }
}
