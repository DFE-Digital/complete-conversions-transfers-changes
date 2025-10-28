using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AccuracyOfHigherNeedsTask
{
    public class AccuracyOfHigherNeedsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AccuracyOfHigherNeedsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAccuracyOfHigherNeeds)
    {
        [BindProperty]
        public bool? ConfirmNumber { get; set; }
        [BindProperty]
        public bool? ConfirmPublishedNumber { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            ConfirmNumber = ConversionTaskData.CheckAccuracyOfHigherNeedsConfirmNumber;
            ConfirmPublishedNumber = ConversionTaskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var result = await Sender.Send(new UpdateAccuracyOfHigherNeedsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, ConfirmNumber, ConfirmPublishedNumber));
            return OnPostProcessResponse(result);
        }
    }
}
