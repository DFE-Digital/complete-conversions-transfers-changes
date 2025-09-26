using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmAcademyOpenedDateTask
{
    public class ConfirmAcademyOpenedDateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmAcademyOpenedDateTaskModel> logger, ErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmAcademyOpenedDate)
    {
        [BindProperty(Name = "opened-date")]
        [DisplayName("Opened academy date")]
        public DateOnly? OpenedDate { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            OpenedDate = ConversionTaskData.ConfirmDateAcademyOpenedDateOpened;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if(OpenedDate?.ToDateTime(new TimeOnly()) > DateTime.Today)
            {
                ModelState.AddModelError(nameof(OpenedDate), string.Format(ValidationConstants.MustBePastDate, "Opened academy date"));
            }
            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }
            await sender.Send(new UpdateConfirmAcademyOpenedDateTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, OpenedDate));
            TempData.SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
