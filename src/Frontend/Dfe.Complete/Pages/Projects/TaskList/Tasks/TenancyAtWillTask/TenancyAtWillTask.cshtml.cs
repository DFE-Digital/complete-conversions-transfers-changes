using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.TenancyAtWillTask
{
    public class TenancyAtWillTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<TenancyAtWillTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.TenancyAtWill, projectPermissionService)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public bool? EmailSigned { get; set; }

        [BindProperty]
        public bool? ReceiveSigned { get; set; }

        [BindProperty]
        public bool? SaveSigned { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = ConversionTaskData.TenancyAtWillNotApplicable;
            EmailSigned = ConversionTaskData.TenancyAtWillEmailSigned;
            ReceiveSigned = ConversionTaskData.TenancyAtWillReceiveSigned;
            SaveSigned = ConversionTaskData.TenancyAtWillSaveSigned;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await Sender.Send(new UpdateTenancyAtWillTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!,
                NotApplicable, EmailSigned, SaveSigned, ReceiveSigned));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
