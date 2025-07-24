using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask
{
    public class HandoverWithDeliveryOfficerTaskModel(ISender sender, ILogger<HandoverWithDeliveryOfficerTaskModel> _logger) : BaseProjectPageModel(sender, _logger)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "review-project-information")]
        public bool? ReviewProjectInformation { get; set; }
        
        [BindProperty(Name = "make-notes")]
        public bool? MakeNotes { get; set; }

        [BindProperty(Name = "attend-handover-meeting")]
        public bool? AttendHandoverMeeting { get; set; }

        public async Task<IActionResult> OnPost()
        {
            _ = await sender.Send(new UpdateHandoverWithDeliveryOfficerCommand(new ProjectId(Guid.Parse(ProjectId)), NotApplicable, ReviewProjectInformation, MakeNotes, AttendHandoverMeeting));
            return Redirect(string.Format(RouteConstants.ProjectHandoverWithDeliveryOfficerTask, ProjectId));
        }
    }
}
