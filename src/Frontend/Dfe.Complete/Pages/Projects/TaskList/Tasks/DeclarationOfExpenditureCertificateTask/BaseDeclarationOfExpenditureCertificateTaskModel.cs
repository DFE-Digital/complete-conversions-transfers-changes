using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeclarationOfExpenditureCertificateTask
{
    public class BaseDeclarationOfExpenditureCertificateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger logger, NoteTaskIdentifier noteTaskIdentifier, IErrorService errorService)
    : BaseProjectTaskModel(sender, authorizationService, logger, noteTaskIdentifier)
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

        public async Task<IActionResult> OnPost()
        {
            if (!NotApplicable.HasValue && !(ReceivedDate?.ToDateTime(new TimeOnly()) < DateTime.Today))
            {
                ModelState.AddModelError("received-date", string.Format(ValidationConstants.DateInPast, "Received"));
            }
            if (!ModelState.IsValid)
            {
                await base.OnGetAsync();
                errorService.AddErrors(ModelState);
                return Page();
            }
            await Sender.Send(new UpdateDeclarationOfExpenditureCertificateTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, ReceivedDate, NotApplicable, CheckCertificate, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
