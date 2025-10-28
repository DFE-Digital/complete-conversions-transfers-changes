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
    public class RedactAndSendDocumentsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.RedactAndSendDocuments)
    {
        [BindProperty(Name = "redact")]
        public bool? Redact { get; set; }

        [BindProperty(Name = "send")]
        public bool? Send { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "send_to_efsa")]
        public bool? SendToEsfa { get; set; }

        [BindProperty(Name = "send_to_solicitors")]
        public bool? SendToSolicitors { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty(Name = "type")]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                Redact = TransferTaskData.RedactAndSendDocumentsRedact;
                Saved = TransferTaskData.RedactAndSendDocumentsSaved;
                SendToEsfa = TransferTaskData.RedactAndSendDocumentsSendToEsfa;
                Send = TransferTaskData.RedactAndSendDocumentsSendToFundingTeam;
                SendToSolicitors = TransferTaskData.RedactAndSendDocumentsSendToSolicitors;
            }
            else
            {
                Redact = ConversionTaskData.RedactAndSendRedact;
                Saved = ConversionTaskData.RedactAndSendSaveRedaction;
                Send = ConversionTaskData.RedactAndSendSendRedaction;
                SendToSolicitors = ConversionTaskData.RedactAndSendSendSolicitors;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var result = await sender.Send(new UpdateRedactAndSendDocumentsTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, Redact, Saved, SendToEsfa, Send, SendToSolicitors));
            return OnPostProcessResponse(result);
        }
    }
}
