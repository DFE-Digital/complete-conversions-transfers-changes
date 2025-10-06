using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.CommercialTransferAgreementTask
{
    public class CommercialTransferAgreementTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<CommercialTransferAgreementTaskModel> logger)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CommercialTransferAgreement)
    {
        [BindProperty(Name = "agreed")]
        public bool? Agreed { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "questionsreceived")]
        public bool? QuestionsReceived { get; set; }        

        [BindProperty(Name = "questionschecked")]
        public bool? QuestionsChecked { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty]
        public ProjectType? Type { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;

            if (Project.Type == ProjectType.Transfer)
            {
                Agreed = TransferTaskData.CommercialTransferAgreementConfirmAgreed;
                Signed = TransferTaskData.CommercialTransferAgreementConfirmSigned;
                Saved = TransferTaskData.CommercialTransferAgreementSaveConfirmationEmails;
                QuestionsReceived = TransferTaskData.CommercialTransferAgreementQuestionsReceived;
                QuestionsChecked = TransferTaskData.CommercialTransferAgreementQuestionsChecked;
            }
            else
            {
                Agreed = ConversionTaskData.CommercialTransferAgreementAgreed;
                Signed = ConversionTaskData.CommercialTransferAgreementSigned;
                Saved = ConversionTaskData.CommercialTransferAgreementSaved;
                QuestionsReceived = ConversionTaskData.CommercialTransferAgreementQuestionsReceived;
                QuestionsChecked = ConversionTaskData.CommercialTransferAgreementQuestionsChecked;               
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateCommercialAgreementTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, Agreed, Signed, QuestionsReceived, QuestionsChecked, Saved));            
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
