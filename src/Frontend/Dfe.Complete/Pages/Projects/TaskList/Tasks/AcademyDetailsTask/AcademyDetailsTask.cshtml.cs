using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyDetailsTask
{
    public class AcademyDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AcademyDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.AcademyDetails)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
