using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ProcessConversionSupportGrant
{
    public class ProcessConversionSupportGrantModel(ISender sender, IAuthorizationService authorizationService, ILogger<ProcessConversionSupportGrantModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ProcessConversionSupportGrant)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "checked")]
        public bool? ConversionGrantCheckVendorAccount { get; set; }

        [BindProperty(Name = "received")]
        public bool? ConversionGrantPaymentForm { get; set; }

        [BindProperty(Name = "sent")]
        public bool? ConversionGrantSendInformation { get; set; }

        [BindProperty(Name = "shared")]
        public bool? ConversionGrantSharePaymentDate { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);


            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.ConversionGrantNotApplicable;
            ConversionGrantCheckVendorAccount = ConversionTaskData.ConversionGrantCheckVendorAccount;
            ConversionGrantPaymentForm = ConversionTaskData.ConversionGrantPaymentForm;
            ConversionGrantSendInformation = ConversionTaskData.ConversionGrantSendInformation; 
            ConversionGrantSharePaymentDate = ConversionTaskData.ConversionGrantSharePaymentDate;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateProcessSupportGrantTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, ConversionGrantCheckVendorAccount, ConversionGrantPaymentForm, ConversionGrantSendInformation, ConversionGrantSharePaymentDate));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
