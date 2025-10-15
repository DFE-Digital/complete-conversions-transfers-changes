using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChange
{
    public class ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChangeModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChangeModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChange)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
