using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ProcessConversionSupportGrant
{
    public class ProcessConversionSupportGrantModel(ISender sender, IAuthorizationService authorizationService, ILogger<ProcessConversionSupportGrantModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ProcessConversionSupportGrant)
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
