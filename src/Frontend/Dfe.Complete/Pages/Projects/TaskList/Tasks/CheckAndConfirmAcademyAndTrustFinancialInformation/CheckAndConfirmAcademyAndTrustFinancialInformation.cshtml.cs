using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.CheckAndConfirmAcademyAndTrustFinancialInformation
{
    public class CheckAndConfirmAcademyAndTrustFinancialInformationModel(ISender sender, IAuthorizationService authorizationService, ILogger<CheckAndConfirmAcademyAndTrustFinancialInformationModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAndConfirmAcademyAndTrustFinancialInformation)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
