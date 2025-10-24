using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.StakeholderKickoffTask
{
    public class StakeholderKickoffTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<StakeholderKickoffTaskModel> logger, IErrorService errorService)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.StakeholderKickoff)
    {
        [BindProperty(Name = "send-intro-emails")]
        public bool? SendIntroEmails { get; set; }

        [BindProperty(Name = "local-authority-proforma")]
        public bool? LocalAuthorityProforma { get; set; }

        [BindProperty(Name = "local-authority-able-to-convert")]
        public bool? LocalAuthorityAbleToConvert { get; set; }

        [BindProperty(Name = "send-invites")]
        public bool? SendInvites { get; set; }

        [BindProperty(Name = "host-meeting-or-call")]
        public bool? HostMeetingOrCall { get; set; }

        [BindProperty(Name = "significant-date")]
        [DisplayName("The Significant date")]
        public DateOnly? SignificantDate { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync(); 
            var isConversion = Project.Type == ProjectType.Conversion;

            if (isConversion)
            { 
                SendIntroEmails = ConversionTaskData.StakeholderKickOffIntroductoryEmails;
                SendInvites = ConversionTaskData.StakeholderKickOffSetupMeeting;
                HostMeetingOrCall = ConversionTaskData.StakeholderKickOffMeeting;
                LocalAuthorityProforma = ConversionTaskData.StakeholderKickOffLocalAuthorityProforma;
                LocalAuthorityAbleToConvert = ConversionTaskData.StakeholderKickOffCheckProvisionalConversionDate;
            }
            else
            { 
                SendIntroEmails = TransferTaskData.StakeholderKickOffIntroductoryEmails;
                SendInvites = TransferTaskData.StakeholderKickOffSetupMeeting;
                HostMeetingOrCall = TransferTaskData.StakeholderKickOffMeeting;
            }
            
            SignificantDate = Project.SignificantDateProvisional is true ? null : Project.SignificantDate;

            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            await base.OnGetAsync(); 
            var errorToRemove = "SignificantDate must include a month and year";
            ModelState.RemoveError("significant-date", errorToRemove); 

            if (SignificantDate.HasValue && SignificantDate?.ToDateTime(new TimeOnly()) < DateTime.Today)
            {
                ModelState.AddModelError(nameof(SignificantDate), "The Significant date must be in the future.");
            }

            if (!ModelState.IsActuallyValid())
            {
                errorService.AddErrors(ModelState);
                return Page();
            }
            
            var request = new UpdateExternalStakeholderKickOffTaskCommand(Project.Id, 
                SendIntroEmails, 
                LocalAuthorityProforma, 
                LocalAuthorityAbleToConvert, 
                SendInvites, 
                HostMeetingOrCall, 
                SignificantDate,
                User.Identity!.Name);
            
            await Sender.Send(request);
            
            SetTaskSuccessNotification();
            
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        } 
    }
}
