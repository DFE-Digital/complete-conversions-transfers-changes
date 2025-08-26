using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfVariation
{
    public class DeedOfVariationModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeedOfVariationModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfVariation)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }

        [BindProperty(Name = "signed_secretary_state")]
        public bool? SignedSecretaryState { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                NotApplicable = TransferTaskData.DeedOfVariationNotApplicable;
                Received = TransferTaskData.DeedOfVariationReceived;
                Cleared = TransferTaskData.DeedOfVariationCleared;
                Saved = TransferTaskData.DeedOfVariationSaved;
                Sent = TransferTaskData.DeedOfVariationSent;
                Signed = TransferTaskData.DeedOfVariationSigned;
                SignedSecretaryState = TransferTaskData.DeedOfVariationSignedSecretaryState;
            }
            else
            {
                NotApplicable = ConversionTaskData.DeedOfVariationNotApplicable;
                Received = ConversionTaskData.DeedOfVariationReceived;
                Cleared = ConversionTaskData.DeedOfVariationCleared;
                Sent = ConversionTaskData.DeedOfVariationSent;
                Saved = ConversionTaskData.DeedOfVariationSaved;
                Signed = ConversionTaskData.DeedOfVariationSigned;
                SignedSecretaryState = ConversionTaskData.DeedOfVariationSignedSecretaryState;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateDeedOfVariationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, Received, Cleared, Sent, Saved, Signed, SignedSecretaryState, NotApplicable));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
