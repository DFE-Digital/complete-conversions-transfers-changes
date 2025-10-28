using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyRiskProtectionArrangementsTask
{
    public class TransferAcademyRiskProtectionArrangementsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<TransferAcademyRiskProtectionArrangementsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmRiskProtectionArrangementsPolicy)
    {
        [BindProperty]
        public bool? RpaPolicyConfirm { get; set; }
         
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            RpaPolicyConfirm = TransferTaskData.RpaPolicyConfirm;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var result = await Sender.Send(new UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, RpaPolicyConfirm, null, null));
            return OnPostProcessResponse(result);
        }
    }
}
