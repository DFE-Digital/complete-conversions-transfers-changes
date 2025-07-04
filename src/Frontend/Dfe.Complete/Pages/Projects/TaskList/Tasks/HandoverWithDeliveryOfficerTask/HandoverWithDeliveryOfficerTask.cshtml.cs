using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask
{
    public class HandoverWithDeliveryOfficerTaskModel(ISender sender, IAuthorizationService authorizationService) : ProjectTaskBaseModel(sender, authorizationService)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "review-project-information")]
        public bool? ReviewProjectInformation { get; set; }
        
        [BindProperty(Name = "make-notes")]
        public bool? MakeNotes { get; set; }

        [BindProperty(Name = "attend-handover-meeting")]
        public bool? AttendHandoverMeeting { get; set; }
    }
}
