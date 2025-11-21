using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandQuestionnaireTask
{
    public class LandQuestionnaireTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LandQuestionnaireTaskModel> logger)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LandQuestionnaire)
    {
        [BindProperty]
        public bool? Cleared { get; set; }

        [BindProperty]
        public bool? Received { get; set; }

        [BindProperty]
        public bool? Signed { get; set; }

        [BindProperty]
        public bool? Saved { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            Received = ConversionTaskData.LandQuestionnaireReceived;
            Cleared = ConversionTaskData.LandQuestionnaireCleared;
            Signed = ConversionTaskData.LandQuestionnaireSigned;
            Saved = ConversionTaskData.LandQuestionnaireSaved;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateLandQuestionnaireTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Received, Cleared, Signed, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
