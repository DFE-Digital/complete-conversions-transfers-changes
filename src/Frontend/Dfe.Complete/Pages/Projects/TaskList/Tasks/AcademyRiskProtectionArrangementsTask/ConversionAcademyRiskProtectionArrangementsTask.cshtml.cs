using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyRiskProtectionArrangementsTask
{
    public class ConversionAcademyRiskProtectionArrangementsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConversionAcademyRiskProtectionArrangementsTaskModel> logger, IErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmRiskProtectionArrangements)
    {
        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.RiskProtectionArrangementOptionRequired)]
        public RiskProtectionArrangementOption? RpaOption { get; set; }
        [BindProperty]
        public string? RpaReason { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            RpaOption = ConversionTaskData.RiskProtectionArrangementOption;
            RpaReason = ConversionTaskData.RiskProtectionArrangementReason;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (RpaOption == RiskProtectionArrangementOption.Commercial && string.IsNullOrWhiteSpace(RpaReason))
            {
                ModelState.AddModelError(nameof(RpaReason), ValidationConstants.RequiredSummary);
            }
            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }
            await Sender.Send(new UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, null, RpaOption, RpaReason));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
