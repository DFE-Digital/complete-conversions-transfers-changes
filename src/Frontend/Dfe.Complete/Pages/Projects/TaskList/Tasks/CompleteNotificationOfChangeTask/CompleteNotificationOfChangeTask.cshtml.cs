using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.CompleteNotificationOfChangeTask
{
    public class CompleteNotificationOfChangeTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<CompleteNotificationOfChangeTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CompleteNotificationOfChange)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        
        [BindProperty(Name = "notApplicable")]
        public bool? NotApplicable { get; set; }
        
        [BindProperty(Name = "tellLocalAuthority")]
        public bool? TellLocalAuthority { get; set; }
        
        [BindProperty(Name = "checkDocument")]
        public bool? CheckDocument { get; set; }
        
        [BindProperty(Name = "sendDocument")]
        public bool? SendDocument { get; set; }
        
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = ConversionTaskData.CompleteNotificationOfChangeNotApplicable;
            TellLocalAuthority = ConversionTaskData.CompleteNotificationOfChangeTellLocalAuthority;
            CheckDocument = ConversionTaskData.CompleteNotificationOfChangeCheckDocument;
            SendDocument = ConversionTaskData.CompleteNotificationOfChangeSendDocument;
            
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {            
            var result = await Sender.Send(new UpdateCompleteNotificationOfChangeTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, TellLocalAuthority, CheckDocument, SendDocument));
            return OnPostProcessResponse(result);
        } 
    } 
}
