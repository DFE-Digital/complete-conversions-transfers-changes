using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.StakeholderKickoffTask
{
    public class StakeholderKickoffTaskModel(ISender sender, ErrorService errorService, ILogger<StakeholderKickoffTaskModel> _logger) : BaseProjectPageModel(sender, _logger)
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
        public DateOnly? SignificantDate { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            var result = await sender.Send(new GetProjectHistoryByProjectIdQuery(ProjectId));
            Project = result.Value;
            var isConversion = Project.Type == ProjectType.Conversion;

            if (isConversion)
            {
                var request =  new GetConversionTasksDataByIdQuery(Project.TasksDataId);
                var taskDataResult = await sender.Send(request);

                var taskData = taskDataResult.Value;

                SendIntroEmails = taskData?.StakeholderKickOffIntroductoryEmails;
                SendInvites = taskData?.StakeholderKickOffSetupMeeting;
                HostMeetingOrCall = taskData?.StakeholderKickOffMeeting;
                LocalAuthorityProforma = taskData?.StakeholderKickOffLocalAuthorityProforma;
                LocalAuthorityAbleToConvert = taskData?.StakeholderKickOffCheckProvisionalConversionDate;
            }
            else
            {
                var request =  new GetTransferTasksDataByIdQuery(Project.TasksDataId);
                var taskDataResult = await sender.Send(request);

                var taskData = taskDataResult.Value;

                SendIntroEmails = taskData?.StakeholderKickOffIntroductoryEmails;
                SendInvites = taskData?.StakeholderKickOffSetupMeeting;
                HostMeetingOrCall = taskData?.StakeholderKickOffMeeting;
            }
            
            SignificantDate = Project.SignificantDateProvisional is true ? null : Project.SignificantDate;

            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            await base.OnGetAsync();
            
            if (SignificantDate.HasValue && SignificantDate?.ToDateTime(new TimeOnly()) < DateTime.Today)
            {
                ModelState.AddModelError(nameof(SignificantDate), "The Significant date must be in the future.");
            }
            
            if (SignificantDate.HasValue && !ModelState.IsValid)
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
                User.Identity.Name);
            
            await sender.Send(request);
            
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
