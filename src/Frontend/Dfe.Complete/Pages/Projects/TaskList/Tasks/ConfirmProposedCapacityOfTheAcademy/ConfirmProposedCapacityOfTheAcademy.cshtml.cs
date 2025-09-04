using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.ArticlesOfAssociationTask;
using Dfe.Complete.Services;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmProposedCapacityOfTheAcademy
{
    public class ConfirmProposedCapacityOfTheAcademyModel(ISender sender, IAuthorizationService authorizationService, ILogger<ArticlesOfAssociationTaskModel> logger, ErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmProposedCapacityOfTheAcademy)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "reception-to-six-years")]
        [RequiredIfNotApplicableIsNotSet("NotApplicable", ErrorMessage = ValidationConstants.ReceptionToSixYears)]
        [ValidNumber(0, int.MaxValue, "NotApplicable", ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string ReceptionToSixYears { get; set; } = "";
        [BindProperty(Name = "seven-to-eleven-years")]
        [RequiredIfNotApplicableIsNotSet("NotApplicable", ErrorMessage = ValidationConstants.SevenToElevenYears)]
        [ValidNumber(0, int.MaxValue, "NotApplicable",ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string SevenToElevenYears { get; set; } = "";

        [BindProperty(Name = "twelve-or-above-years")]
        [RequiredIfNotApplicableIsNotSet("NotApplicable", ErrorMessage = ValidationConstants.TwelveOrAboveYears)]
        [ValidNumber(0, int.MaxValue, "NotApplicable", ErrorMessage = ValidationConstants.ProposedCapacityMustBeNumber)]
        public string TwelveOrAboveYears { get; set; } = "";

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.ProposedCapacityOfTheAcademyNotApplicable;
            ReceptionToSixYears = ConversionTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears!;
            SevenToElevenYears = ConversionTaskData.ProposedCapacityOfTheAcademySevenToElevenYears!;
            TwelveOrAboveYears = ConversionTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears!;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }
            await sender.Send(new UpdateConfirmProposedCapacityOfTheAcademyTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, ReceptionToSixYears, SevenToElevenYears, TwelveOrAboveYears));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
