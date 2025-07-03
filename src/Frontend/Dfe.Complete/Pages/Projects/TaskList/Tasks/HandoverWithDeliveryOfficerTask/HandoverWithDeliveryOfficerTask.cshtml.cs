using Dfe.Complete.Application.Projects.Commands.ConversionTasks;
using Dfe.Complete.Application.Projects.Queries.ConversionTasks;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;

public class HandoverWithDeliveryOfficerTaskModel(ISender sender) : ProjectTaskModel(sender)
{
    [BindProperty(Name = "not-applicable")]
    public bool NotApplicable { get; set; }

    [BindProperty(Name = "review-project-information")]
    public bool ReviewProjectInformation { get; set; }

    [BindProperty(Name = "make-notes")]
    public bool MakeNotes { get; set; }

    [BindProperty(Name = "attend-handover-meeting")]
    public bool AttendHandoverMeeting { get; set; }


    public async override Task<IActionResult> OnGetAsync()
    {
        TaskIdentifier = "handover";
        await base.OnGetAsync();

        var response = await sender.Send(new GetHandoverTaskDataByProjectIdQuery(new ProjectId(new Guid(ProjectId))));
        if (!response.IsSuccess || response.Value == null)
            throw new InvalidOperationException($"Failed to retrieve handover task data: {response.Error}");

        var taskData = response.Value;
        NotApplicable = taskData.NotApplicable ?? false;
        ReviewProjectInformation = taskData.ReviewProjectInformation ?? false;
        MakeNotes = taskData.MakeNotes ?? false;
        AttendHandoverMeeting = taskData.AttendHandoverMeeting ?? false;

        return Page();
    }


    public async override Task<IActionResult> OnPostAsync()
    {
        await sender.Send(new UpdateHandoverTaskDataByProjectIdCommand(
            new ProjectId(new Guid(ProjectId)),
            NotApplicable,
            ReviewProjectInformation,
            MakeNotes,
            AttendHandoverMeeting
        ));

        return Page();
        // return Task.FromResult<IActionResult>(RedirectToPage("/Projects/TaskList/TaskList", new { projectId = ProjectId }));
    }
}
