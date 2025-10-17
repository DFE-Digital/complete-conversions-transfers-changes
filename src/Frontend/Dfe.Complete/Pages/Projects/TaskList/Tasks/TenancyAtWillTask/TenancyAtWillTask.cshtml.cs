using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.TenancyAtWillTask
{
    public class TenancyAtWillTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<TenancyAtWillTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.TenancyAtWill)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
