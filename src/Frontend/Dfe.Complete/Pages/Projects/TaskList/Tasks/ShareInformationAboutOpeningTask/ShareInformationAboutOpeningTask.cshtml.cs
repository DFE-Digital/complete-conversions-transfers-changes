using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ShareInformationAboutOpeningTask
{
    public class ShareInformationAboutOpeningTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ShareInformationAboutOpeningTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAccuracyOfHigherNeeds)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
