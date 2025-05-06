using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.La.HandoverWithDeliveryOfficerTask
{
    public class HandoverWithDeliveryOfficerTaskModel(ISender sender) : BaseProjectPageModel(sender)
    {

        [BindProperty(SupportsGet = true, Name = "projectId")]
        public string ProjectId { get; set; }

        public string SchoolName { get; set; }

        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "review-project-information")]
        public bool? ReviewProjectInformation { get; set; }
        
        [BindProperty(Name = "make-notes")]
        public bool? MakeNotes { get; set; }

        [BindProperty(Name = "attend-handover-meeting")]
        public bool? AttendHandoverMeeting { get; set; }

   
        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            return Redirect(string.Format(RouteConstants.ProjectHandoverWithDeliveryOfficerTask, ProjectId));
        }
    }
}
