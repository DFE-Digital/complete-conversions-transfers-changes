using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Queries.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;

public class HandoverWithDeliveryOfficerTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.Handover)
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
        await base.OnGetAsync();
        var result = await sender.Send(new GetTaskDataByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId))));
        if (result.IsSuccess && result.Value != null)
        {
            ReviewProjectInformation = result.Value.HandoverReview;
            MakeNotes = result.Value.HandoverNotes;
            AttendHandoverMeeting = result.Value.HandoverMeeting;
            NotApplicable = result.Value.HandoverNotApplicable;
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        _ = await sender.Send(new UpdateHandoverWithDeliveryOfficerCommand(new ProjectId(Guid.Parse(ProjectId)), NotApplicable, ReviewProjectInformation, MakeNotes, AttendHandoverMeeting));
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}