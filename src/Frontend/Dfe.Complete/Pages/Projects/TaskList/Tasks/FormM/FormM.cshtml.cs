using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.FormM
{
    public class FormMModel(ISender sender, IAuthorizationService authorizationService, ILogger<FormMModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.FormM)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }
        
        [BindProperty(Name = "received")]
        public bool? Received { get; set; }
        
        [BindProperty(Name = "receivedTitlePlans")]
        public bool? ReceivedTitlePlans { get; set; }
        
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; } 
        
        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }
        
        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }
        
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            
            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = TransferTaskData.FormMNotApplicable;
            Received = TransferTaskData.FormMReceivedFormM;
            ReceivedTitlePlans = TransferTaskData.FormMReceivedTitlePlans;
            Cleared = TransferTaskData.FormMCleared;
            Signed = TransferTaskData.FormMSigned;
            Saved = TransferTaskData.FormMSaved;
            
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {            
            await Sender.Send(new UpdateFormMTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, Received, ReceivedTitlePlans, Cleared, Signed, Saved ));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
