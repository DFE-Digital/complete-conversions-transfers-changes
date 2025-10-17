using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ClosureOrTransferDeclaration
{
    public class ClosureOrTransferDeclarationModel(ISender sender, IAuthorizationService authorizationService, ILogger<ClosureOrTransferDeclarationModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ClosureOrTransferDeclaration)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
