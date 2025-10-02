using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models; 
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeclarationOfExpenditureCertificateTask
{
    public class DeclarationOfExpenditureCertificateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeclarationOfExpenditureCertificateTaskModel> logger, IErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeclarationOfExpenditureCertificate)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }
        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "check-certificate")]
        public bool? CheckCertificate { get; set; }

        [BindProperty(Name = "received-date")]
        [DisplayName("Received date")]
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
            NotApplicable = TransferTaskData.DeclarationOfExpenditureCertificateNotApplicable;
            CheckCertificate = TransferTaskData.DeclarationOfExpenditureCertificateCorrect;
            ReceivedDate = TransferTaskData.DeclarationOfExpenditureCertificateDateReceived;
            Saved = TransferTaskData.DeclarationOfExpenditureCertificateSaved;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!NotApplicable.HasValue && ReceivedDate.HasValue && !(ReceivedDate?.ToDateTime(new TimeOnly()) < DateTime.Today))
            {
                ModelState.AddModelError("received-date", string.Format(ValidationConstants.DateInPast, "Received date"));
            }

            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }
            await sender.Send(new UpdateDeclarationOfExpenditureCertificateTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, ReceivedDate, NotApplicable, CheckCertificate, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
