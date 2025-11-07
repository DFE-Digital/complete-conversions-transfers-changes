using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.TrustModificationOrderTask
{
    public class TrustModificationOrderTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<TrustModificationOrderTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.TrustModificationOrder)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if(Project.TasksDataType != TaskType.Conversion)
            {
                return Redirect(RouteConstants.ErrorPage);
            }

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.TrustModificationOrderNotApplicable;
            Received = ConversionTaskData.TrustModificationOrderReceived;
            Cleared = ConversionTaskData.TrustModificationOrderCleared;
            Saved = ConversionTaskData.TrustModificationOrderSaved;
            Sent = ConversionTaskData.TrustModificationOrderSentLegal;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateTrustModificationOrderTaskCommand(new Domain.ValueObjects.TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, Received, Sent, Cleared, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
