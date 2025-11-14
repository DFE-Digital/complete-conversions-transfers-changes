using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DirectionToTransfer
{
    public class DirectionToTransferModel(ISender sender, IAuthorizationService authorizationService, ILogger<DirectionToTransferModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DirectionToTransfer)
    {
        
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        
        
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }
        
        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

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
            
            NotApplicable = ConversionTaskData.DirectionToTransferNotApplicable;
            Received = ConversionTaskData.DirectionToTransferReceived;
            Cleared = ConversionTaskData.DirectionToTransferCleared;
            Signed = ConversionTaskData.DirectionToTransferSigned;
            Saved = ConversionTaskData.DirectionToTransferSaved;

            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {            
            await Sender.Send(new UpdateDirectionToTransferTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, Received, Cleared, Signed, Saved ));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
