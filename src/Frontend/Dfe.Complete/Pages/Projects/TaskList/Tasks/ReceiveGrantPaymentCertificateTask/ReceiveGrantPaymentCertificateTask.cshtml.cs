using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ReceiveGrantPaymentCertificateTask
{
    public class ReceiveGrantPaymentCertificateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ReceiveGrantPaymentCertificateTaskModel> logger, ErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ReceiveGrantPaymentCertificate)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }
        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "check-certificate")]
        public bool? CheckCertificate { get; set; }

        [BindProperty(Name = "received-date")]
        public DateOnly? ReceivedDate { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.ReceiveGrantPaymentCertificateNotApplicable;
            CheckCertificate = ConversionTaskData.ReceiveGrantPaymentCertificateCheckCertificate;
            ReceivedDate = ConversionTaskData.ReceiveGrantPaymentCertificateDateReceived;
            Saved = ConversionTaskData.ReceiveGrantPaymentCertificateSaveCertificate;

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
            await sender.Send(new UpdateDeclarationOfExpenditureCertificateTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, ReceivedDate, NotApplicable, CheckCertificate, Saved));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
