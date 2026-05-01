using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Validation;
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
    public class LAConfirmsPayrollDeadlineTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LAConfirmsPayrollDeadlineTaskModel> logger, IErrorService errorService, IProjectPermissionService projectPermissionService, ISignificantDateValidator significantDateValidator)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LAConfirmsPayrollDeadline, projectPermissionService)
    {
        private readonly ISignificantDateValidator _significantDateValidator = significantDateValidator;
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
            PayrollDeadline = ConversionTaskData.LAPayrollDeadline;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await base.OnGetAsync();

            // Use domain validator for payroll deadline validation
            if (PayrollDeadline.HasValue)
            {
                var validationResult = _significantDateValidator.ValidatePayrollDeadline(PayrollDeadline, Project);
                if (!validationResult.IsValid)
                {
                    ModelState.AddModelError("payroll-deadline", validationResult.ErrorMessage!);
                }
            }

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            await Sender.Send(new UpdateLAConfirmsPayrollDeadlineTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, PayrollDeadline));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}