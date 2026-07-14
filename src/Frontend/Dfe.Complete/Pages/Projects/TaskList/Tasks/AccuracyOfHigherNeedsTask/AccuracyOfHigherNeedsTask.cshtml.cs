using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AccuracyOfHigherNeedsTask
{
    public class AccuracyOfHigherNeedsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AccuracyOfHigherNeedsTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAccuracyOfHigherNeeds, projectPermissionService)
    {
        private const string OptionConfirmPublishedNumber = "confirm-published-number";
        private const string OptionConfirmNumber = "confirm-number";
        private const string OptionCheckReturnedForm = "check-returned-form";
        private const string OptionSendForm = "send-form";

        [BindProperty]
        public List<string> SelectedOptions { get; set; } = [];

        [BindProperty]
        public bool? ConfirmNumber { get; set; }

        [BindProperty]
        public bool? ConfirmPublishedNumber { get; set; }
    
        [BindProperty]
        public bool? CheckReturnedForm { get; set; }

        [BindProperty]
        public bool? SendForm { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            ConfirmNumber = ConversionTaskData.CheckAccuracyOfHigherNeedsConfirmNumber;
            ConfirmPublishedNumber = ConversionTaskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber;
            CheckReturnedForm = ConversionTaskData.CheckAccuracyOfHigherNeedsCheckReturnedForm;
            SendForm = ConversionTaskData.CheckAccuracyOfHigherNeedsSendForm;

            SelectedOptions = [];
            if (ConfirmPublishedNumber == true)
                SelectedOptions.Add(OptionConfirmPublishedNumber);
            if (ConfirmNumber == true)
                SelectedOptions.Add(OptionConfirmNumber);
            if (CheckReturnedForm == true)
                SelectedOptions.Add(OptionCheckReturnedForm);
            if (SendForm == true)
                SelectedOptions.Add(OptionSendForm);

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            SelectedOptions ??= [];

            ConfirmPublishedNumber = SelectedOptions.Contains(OptionConfirmPublishedNumber);
            ConfirmNumber = SelectedOptions.Contains(OptionConfirmNumber);
            CheckReturnedForm = SelectedOptions.Contains(OptionCheckReturnedForm);
            SendForm = SelectedOptions.Contains(OptionSendForm);

            await Sender.Send(new UpdateAccuracyOfHigherNeedsTaskCommand(new TaskDataId(
                TasksDataId.GetValueOrDefault())!,
                ConfirmNumber,
                ConfirmPublishedNumber,
                CheckReturnedForm,
                SendForm
            ));

            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
