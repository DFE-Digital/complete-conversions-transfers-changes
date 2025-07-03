using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects;

public class ProjectTaskModel(ISender sender) : BaseProjectPageModel(sender)
{
    [BindProperty(SupportsGet = true, Name = "task_identifier")]
    [Required]
    public required string TaskIdentifier { get; set; }
    public List<NoteDto> Notes { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult)
        {
            return baseResult;
        }

        NoteTaskIdentifier? validTaskIdentifier = null;
        if (TaskIdentifier != null)
        {
            validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);
            if (validTaskIdentifier == null)
                return NotFound();
        }

        SchoolName = Establishment?.Name ?? string.Empty;
        Title = $"{validTaskIdentifier}";

        var noteQuery = new GetProjectTaskNotesByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId.ToString())), (NoteTaskIdentifier)validTaskIdentifier!);
        var response = await sender.Send(noteQuery);
        Notes = response.IsSuccess && response.Value != null ? response.Value : [];

        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("Success"));
    }

    public string GetTaskName(string taskIdentifier)
    {
        var validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(taskIdentifier);

        return validTaskIdentifier switch
        {
            NoteTaskIdentifier.Handover => "TaskList/Tasks/HandoverWithDeliveryOfficerTask/_HandoverWithDeliveryOfficerTask",
            NoteTaskIdentifier.StakeholderKickOff => "TaskList/Tasks/StakeholderKickOffTask/_StakeholderKickOffTask",
            NoteTaskIdentifier.RiskProtectionArrangement => "TaskList/Tasks/RiskProtectionArrangementTask/_RiskProtectionArrangementTask",
            NoteTaskIdentifier.CheckAccuracyOfHigherNeeds => "TaskList/Tasks/CheckAccuracyOfHigherNeedsTask/_CheckAccuracyOfHigherNeedsTask",
            NoteTaskIdentifier.CompleteNotificationOfChange => "TaskList/Tasks/CompleteNotificationOfChangeTask/_CompleteNotificationOfChangeTask"
        };
    }

    public object GetTaskSpecificModel()
    {
        var validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);

        return validTaskIdentifier switch
        {
            NoteTaskIdentifier.Handover => new TaskList.Tasks.HandoverWithDeliveryOfficerTask.HandoverWithDeliveryOfficerTaskModel(sender) { TaskIdentifier = this.TaskIdentifier, Project = this.Project },
            NoteTaskIdentifier.StakeholderKickOff => new TaskList.Tasks.StakeholderKickOffTask.StakeholderKickOffTaskModel(sender) { TaskIdentifier = this.TaskIdentifier, Project = this.Project },
            NoteTaskIdentifier.RiskProtectionArrangement => new TaskList.Tasks.RiskProtectionArrangementTask.RiskProtectionArrangementTaskModel(sender) { TaskIdentifier = this.TaskIdentifier, Project = this.Project },
            NoteTaskIdentifier.CheckAccuracyOfHigherNeeds => new TaskList.Tasks.CheckAccuracyOfHigherNeedsTask.CheckAccuracyOfHigherNeedsTaskModel(sender) { TaskIdentifier = this.TaskIdentifier, Project = this.Project },
            NoteTaskIdentifier.CompleteNotificationOfChange => new TaskList.Tasks.CompleteNotificationOfChangeTask.CompleteNotificationOfChangeTaskModel(sender) { TaskIdentifier = this.TaskIdentifier, Project = this.Project }
        };
    }
}
