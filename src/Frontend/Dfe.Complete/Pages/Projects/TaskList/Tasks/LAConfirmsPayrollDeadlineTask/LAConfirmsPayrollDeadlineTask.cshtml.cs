using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LAConfirmsPayrollDeadlineTask
{
    public class LAConfirmsPayrollDeadlineTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LAConfirmsPayrollDeadlineTaskModel> logger, IErrorService errorService, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LAConfirmsPayrollDeadline, projectPermissionService)
    {
        [BindProperty(Name = "payroll-deadline")]
        [DisplayName("Payroll deadline")]
        public DateOnly? PayrollDeadline { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            PayrollDeadline = ConversionTaskData.LAConfirmsPayrollDeadline;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }

            await Sender.Send(new UpdateLAConfirmsPayrollDeadlineTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, PayrollDeadline));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}