using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;

public class HandoverWithDeliveryOfficerTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.Handover, projectPermissionService)
{
    [BindProperty(Name = "not-applicable")]
    public bool? NotApplicable { get; set; }

    [BindProperty(Name = "review-project-information")]
    public bool? ReviewProjectInformation { get; set; }

    [BindProperty(Name = "make-notes")]
    public bool? MakeNotes { get; set; }

    [BindProperty(Name = "confirm-sacre-exemption")]
    public bool? ConfirmSacreExemption { get; set; }

    [BindProperty(Name = "attend-handover-meeting")]
    public bool? AttendHandoverMeeting { get; set; }

    [BindProperty]
    public Guid? TasksDataId { get; set; }
    [BindProperty]
    public ProjectType? Type { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        Type = Project.Type;
        TasksDataId = Project.TasksDataId?.Value;
        if (Project.Type == ProjectType.Transfer)
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
            ConfirmSacreExemption = ConversionTaskData.HandoverConfirmSacreExemption;
            AttendHandoverMeeting = ConversionTaskData.HandoverMeeting;
            NotApplicable = ConversionTaskData.HandoverNotApplicable;
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await Sender.Send(new UpdateHandoverWithDeliveryOfficerTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, ReviewProjectInformation, MakeNotes, ConfirmSacreExemption, AttendHandoverMeeting));
        SetTaskSuccessNotification();
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}