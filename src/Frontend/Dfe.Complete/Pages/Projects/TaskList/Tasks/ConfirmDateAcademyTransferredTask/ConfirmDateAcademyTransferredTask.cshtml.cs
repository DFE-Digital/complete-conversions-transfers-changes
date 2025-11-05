using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;


namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmDateAcademyTransferredTask
{
    public class ConfirmDateAcademyTransferredTaskModel(ISender sender,
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
            await Sender.Send(new UpdateConfirmDateAcademyTransferredTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!, DateAcademyTransferred));

            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
