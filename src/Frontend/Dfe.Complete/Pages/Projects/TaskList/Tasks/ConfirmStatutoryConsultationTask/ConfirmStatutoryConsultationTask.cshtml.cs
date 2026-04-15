using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmStatutoryConsultationTask;

public class ConfirmStatutoryConsultationTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmStatutoryConsultationTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmStatutoryConsultation, projectPermissionService)
{
    [BindProperty(Name = "not-applicable")]
    public bool? NotApplicable { get; set; }

    [BindProperty(Name = "consultation-complete")]
    public bool? ConsultationComplete { get; set; }

    [BindProperty]
    public Guid? TasksDataId { get; set; }
    [BindProperty]
    public ProjectType? Type { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        Type = Project.Type;
        TasksDataId = Project.TasksDataId?.Value;
        
        NotApplicable = ConversionTaskData.StatutoryConsultationNotApplicable;
        ConsultationComplete = ConversionTaskData.StatutoryConsultationComplete;
        
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await Sender.Send(new UpdateConfirmStatutoryConsultationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, ConsultationComplete));
        SetTaskSuccessNotification();
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}