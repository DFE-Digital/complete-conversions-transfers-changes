using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.RedactAndSendDocumentsTask
{
    public class RedactAndSendTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.RedactAndSend)
    {
        [BindProperty(Name = "redact")]
        public bool? Redact { get; set; }

        [BindProperty(Name = "send")]
        public bool? Send { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "send_to_solicitors")]
        public bool? SendToSolicitors { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;

            Redact = ConversionTaskData.RedactAndSendRedact;
            Saved = ConversionTaskData.RedactAndSendSaveRedaction;
            Send = ConversionTaskData.RedactAndSendSendRedaction;
            SendToSolicitors = ConversionTaskData.RedactAndSendSendSolicitors;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateRedactAndSendDocumentsTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!, ProjectType.Conversion, Redact, Saved, null, Send, SendToSolicitors));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
