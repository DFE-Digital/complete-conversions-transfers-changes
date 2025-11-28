using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandConsentLetterTask
{
    public class LandConsentLetterTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LandConsentLetterTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LandConsentLetter)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "drafted")]
        public bool? Drafted { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = TransferTaskData.LandConsentLetterNotApplicable;
            Drafted = TransferTaskData.LandConsentLetterDrafted;
            Signed = TransferTaskData.LandConsentLetterSigned;
            Saved = TransferTaskData.LandConsentLetterSaved;
            Sent = TransferTaskData.LandConsentLetterSent;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateLandConsentLetterTaskCommand(new Domain.ValueObjects.TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, Drafted, Signed, Sent, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
