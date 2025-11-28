using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.ArticlesOfAssociationTask;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmProposedCapacityOfTheAcademyTask
{
    public class ConfirmProposedCapacityOfTheAcademyTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ArticlesOfAssociationTaskModel> logger, IErrorService errorService, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmProposedCapacityOfTheAcademy, projectPermissionService)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "reception-to-six-years")]
        [ValidNumber(0, int.MaxValue, "NotApplicable", ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string? ReceptionToSixYears { get; set; }

        [BindProperty(Name = "seven-to-eleven-years")]
        [ValidNumber(0, int.MaxValue, "NotApplicable", ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string? SevenToElevenYears { get; set; }

        [BindProperty(Name = "twelve-or-above-years")]
        [ValidNumber(0, int.MaxValue, "NotApplicable", ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string? TwelveOrAboveYears { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.ProposedCapacityOfTheAcademyNotApplicable;
            ReceptionToSixYears = ConversionTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears!;
            SevenToElevenYears = ConversionTaskData.ProposedCapacityOfTheAcademySevenToElevenYears!;
            TwelveOrAboveYears = ConversionTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears!;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            ValidateProperty(nameof(TwelveOrAboveYears), TwelveOrAboveYears, ValidationConstants.TwelveOrAboveYears);
            ValidateProperty(nameof(SevenToElevenYears), SevenToElevenYears, ValidationConstants.SevenToElevenYears);
            ValidateProperty(nameof(ReceptionToSixYears), ReceptionToSixYears, ValidationConstants.ReceptionToSixYears);
            if (errorService.HasErrors() || !ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }
            await Sender.Send(new UpdateConfirmProposedCapacityOfTheAcademyTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, ReceptionToSixYears, SevenToElevenYears, TwelveOrAboveYears));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }

        private void ValidateProperty(string name, string? value, string errorMessage)
        {
            var prop = typeof(ConfirmProposedCapacityOfTheAcademyTaskModel).GetProperty(name);
            if (prop == null) return;
            var bindAttr = prop.GetCustomAttribute<BindPropertyAttribute>();
            if (bindAttr != null && NotApplicable != true && string.IsNullOrWhiteSpace(value))
            {
                errorService.AddError(bindAttr.Name!, errorMessage);
            }
        }
    }
}
