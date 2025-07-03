using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;

public class HandoverWithDeliveryOfficerTaskModel(ISender sender) : ProjectTaskModel(sender)
{
    [BindProperty(Name = "not-applicable")]
    public bool? NotApplicable { get; set; }

    [BindProperty(Name = "review-project-information")]
    public bool? ReviewProjectInformation { get; set; }
    
    [BindProperty(Name = "make-notes")]
    public bool? MakeNotes { get; set; }

    [BindProperty(Name = "attend-handover-meeting")]
    public bool? AttendHandoverMeeting { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        return await InitializeTaskDataAsync("handover");
    }

    public override Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Projects/TaskList/TaskList", new { projectId = ProjectId }));
    }
}
