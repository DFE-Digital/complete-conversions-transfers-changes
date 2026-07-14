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
        private const string NotApplicableOption = "not-applicable";
        private const string ConfirmPublishedNumberOption = "confirm-published-number";
        private const string ConfirmNumberOption = "confirm-number";
        private const string CheckReturnedFormOption = "check-returned-form";
        private const string SendFormOption = "send-form";

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
        public bool? NotApplicable { get; set; }

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
            NotApplicable = ConversionTaskData.CheckAccuracyOfHigherNeedsNotApplicable;

            SelectedOptions = CheckboxSelectionHelper.BuildSelectedOptions(
                (NotApplicableOption, NotApplicable),
                (ConfirmPublishedNumberOption, ConfirmPublishedNumber),
                (ConfirmNumberOption, ConfirmNumber),
                (CheckReturnedFormOption, CheckReturnedForm),
                (SendFormOption, SendForm));

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            NotApplicable = CheckboxSelectionHelper.IsSelected(SelectedOptions, NotApplicableOption);
            ConfirmPublishedNumber = CheckboxSelectionHelper.IsSelected(SelectedOptions, ConfirmPublishedNumberOption);
            ConfirmNumber = CheckboxSelectionHelper.IsSelected(SelectedOptions, ConfirmNumberOption);
            CheckReturnedForm = CheckboxSelectionHelper.IsSelected(SelectedOptions, CheckReturnedFormOption);
            SendForm = CheckboxSelectionHelper.IsSelected(SelectedOptions, SendFormOption);

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
