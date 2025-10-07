using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;


namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmDateAcademyTransferredTask
{
    public class ConfirmDateAcademyTransferredTaskModel(ISender sender, ErrorService errorService,
      IAuthorizationService authorizationService, ILogger<ConfirmDateAcademyTransferredTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmDateAcademyTransferred)
    {

        [BindProperty(Name = "date-academy-transferred")]
        [DisplayName("Academy transferred date")]

        public DateOnly? DateAcademyTransferred { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync(); 
            TasksDataId = Project.TasksDataId?.Value;
            DateAcademyTransferred = TransferTaskData.ConfirmDateAcademyTransferredDateTransferred;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            // Normalize to dates only (no time component)
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Invalid if the transferred date is today or in the future
            if (DateAcademyTransferred >= today)
            {
                ModelState.AddModelError(
                    nameof(DateAcademyTransferred),
                    string.Format(ValidationConstants.DateInPast, "Academy transferred"));
            }

            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }

            await sender.Send(new UpdateConfirmDateAcademyTransferredTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!, DateAcademyTransferred));

            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));

        }
    }
}
