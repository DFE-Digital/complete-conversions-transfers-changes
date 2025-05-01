using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.StakeholderKickoffTask
{
    public class StakeholderKickoffTaskModel(ISender sender) : BaseProjectPageModel(sender)
    {
        [BindProperty(SupportsGet = true, Name = "projectId")]
        public Guid ProjectId { get; set; }

        public string SchoolName { get; set; }

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

        [BindProperty(Name = "conversion-date")]
        public DateTime? ConversionDate { get; set; }

        public async Task<IActionResult> OnPost()
        {
            return Redirect(string.Format(RouteConstants.ConversionStakeholderKickoffTask, ProjectId));
        }
    }
}
