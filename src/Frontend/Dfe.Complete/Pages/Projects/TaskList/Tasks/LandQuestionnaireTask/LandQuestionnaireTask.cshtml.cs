using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandQuestionnaireTask
{
    public class LandQuestionnaireTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LandQuestionnaireTaskModel> logger, IProjectPermissionService projectPermissionService)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LandQuestionnaire, projectPermissionService)
    {
        [BindProperty]
        public bool? LandQuestionnaireCleared { get; set; }

        [BindProperty]
        public bool? LandQuestionnaireReceived { get; set; }

        [BindProperty]
        public bool? LandQuestionnaireSigned { get; set; }

        [BindProperty]
        public bool? LandQuestionnaireSaved { get; set; }

        [BindProperty]
        public bool? LandRegistryTitlePlansReceived { get; set; }

        [BindProperty]
        public bool? LandRegistryTitlePlansCleared { get; set; }

        [BindProperty]
        public bool? LandRegistryTitlePlansSaved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            LandQuestionnaireReceived = ConversionTaskData.LandQuestionnaireReceived;
            LandQuestionnaireCleared = ConversionTaskData.LandQuestionnaireCleared;
            LandQuestionnaireSigned = ConversionTaskData.LandQuestionnaireSigned;
            LandQuestionnaireSaved = ConversionTaskData.LandQuestionnaireSaved;
            LandRegistryTitlePlansReceived = ConversionTaskData.LandRegistryReceived;
            LandRegistryTitlePlansCleared = ConversionTaskData.LandRegistryCleared;
            LandRegistryTitlePlansSaved = ConversionTaskData.LandRegistrySaved;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateLandQuestionnaireTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!,
                LandQuestionnaireReceived,
                LandQuestionnaireCleared,
                LandQuestionnaireSigned,
                LandQuestionnaireSaved,
                LandRegistryTitlePlansReceived,
                LandRegistryTitlePlansCleared,
                LandRegistryTitlePlansSaved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
