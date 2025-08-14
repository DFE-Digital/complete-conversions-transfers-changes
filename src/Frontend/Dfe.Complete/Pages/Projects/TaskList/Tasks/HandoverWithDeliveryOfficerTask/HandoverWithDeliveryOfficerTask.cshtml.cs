using Dfe.Complete.Application.Projects.Commands.UpdateProject; 
using Dfe.Complete.Constants; 
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
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

    [BindProperty]
    public Guid? TasksDataId { get; set; }
    [BindProperty]
    public ProjectType? ProjectType { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        ProjectType = Project.Type;
        TasksDataId = Project.TasksDataId?.Value;
        if (Project.Type == Domain.Enums.ProjectType.Transfer)
        {
            ReviewProjectInformation = TransferTaskData.HandoverReview;
            MakeNotes = TransferTaskData.HandoverNotes;
            AttendHandoverMeeting = TransferTaskData.HandoverMeeting;
            NotApplicable = TransferTaskData.HandoverNotApplicable;
        }
        else
        {
            ReviewProjectInformation = ConversionTaskData.HandoverReview;
            MakeNotes = ConversionTaskData.HandoverNotes;
            AttendHandoverMeeting = ConversionTaskData.HandoverMeeting;
            NotApplicable = ConversionTaskData.HandoverNotApplicable;
        }
        return Page();
    } 

    public async Task<IActionResult> OnPost()
    {
        _ = await sender.Send(new UpdateHandoverWithDeliveryOfficerCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, ProjectType, NotApplicable, ReviewProjectInformation, MakeNotes, AttendHandoverMeeting));
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}