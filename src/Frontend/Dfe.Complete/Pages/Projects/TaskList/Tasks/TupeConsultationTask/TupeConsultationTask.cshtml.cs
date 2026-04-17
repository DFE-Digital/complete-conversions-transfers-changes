using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.TupeConsultationTask
{
    public class TupeConsultationTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<TupeConsultationTaskModel> logger, IProjectPermissionService projectPermissionService)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.TupeConsultation, projectPermissionService)
    {
        [BindProperty(Name = "completed")]
        public bool? Completed { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;

            // TUPE Consultation is only for conversion projects
            if (Project.Type == ProjectType.Conversion)
            {
                Completed = ConversionTaskData.TupeConsultationCompleted;
            }
            else
            {
                // Redirect away if not a conversion project
                return Redirect(RouteConstants.ErrorPage);
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateTupeConsultationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, Completed));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}