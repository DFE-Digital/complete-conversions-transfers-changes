using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.SubleasesTask
{
    public class SubleasesTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<SubleasesTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.Subleases)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public bool? Received { get; set; }

        [BindProperty]
        public bool? Cleared { get; set; }

        [BindProperty]
        public bool? Signed { get; set; }

        [BindProperty]
        public bool? Saved { get; set; }

        [BindProperty]
        public bool? EmailSigned { get; set; }

        [BindProperty]
        public bool? SaveSigned { get; set; }
        [BindProperty]
        public bool? ReceiveSigned { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = ConversionTaskData.SubleasesNotApplicable;
            Received = ConversionTaskData.SubleasesReceived;
            Cleared = ConversionTaskData.SubleasesCleared;
            Signed = ConversionTaskData.SubleasesSigned;
            Saved = ConversionTaskData.SubleasesSaved;
            EmailSigned = ConversionTaskData.SubleasesEmailSigned;
            ReceiveSigned = ConversionTaskData.SubleasesReceiveSigned;
            SaveSigned = ConversionTaskData.SubleasesSaveSigned;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Sender.Send(new UpdateSubleasesTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!,
                NotApplicable, Received, Cleared, Signed, Saved, EmailSigned, SaveSigned, ReceiveSigned));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
