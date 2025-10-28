using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmBankDetailsTask
{
    public class ConfirmBankDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmBankDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChange)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty]
        public bool? BankDetailsChangingYesNo { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            BankDetailsChangingYesNo = TransferTaskData.BankDetailsChangingYesNo;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await Sender.Send(new UpdateConfirmBankDetailsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, BankDetailsChangingYesNo));
            return OnPostProcessResponse(result);
        }
    }
}
