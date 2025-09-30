using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Microsoft.AspNetCore.Authorization;
using GovUK.Dfe.CoreLibs.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks;

public class BaseProjectTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger logger, NoteTaskIdentifier taskIdentifier) : BaseProjectPageModel(sender, logger)
{
    public required NoteTaskIdentifier TaskIdentifier { get; set; } = taskIdentifier;
    public IReadOnlyList<NoteDto> Notes { get; private set; } = [];

    public bool CanAddNotes => Project.State != ProjectState.Deleted && Project.State != ProjectState.Completed && Project.State != ProjectState.DaoRevoked;

    public bool CanEditNote(UserId noteUserId)
    {
        if (Project.State == ProjectState.Completed || noteUserId != User.GetUserId())
            return false;
        return true;
    } 

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        var notesResult = await Sender.Send(new GetTaskNotesByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId)), TaskIdentifier));
        if (!notesResult.IsSuccess)
            throw new InvalidOperationException($"Could not load notes for project {ProjectId}");

        Notes = notesResult.Value ?? [];

        await GetProjectTaskDataAsync();
        await SetCurrentUserTeamAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddNoteAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        string? errorMessage = null;
        if (!CanAddNotes)
            errorMessage = "The project is not active and no further notes can be added.";
        else if (!(await authorizationService.AuthorizeAsync(User, UserPolicyConstants.CanAddNotes)).Succeeded)
            errorMessage = "You are not authorised to perform this action.";

        if (errorMessage != null)
        {
            TempData.SetNotification(
                NotificationType.Error,
                "Important",
                errorMessage
            );

            return RedirectToPage(new { projectId = ProjectId });
        }

        return Redirect(string.Format(RouteConstants.ProjectAddTaskNote, ProjectId, TaskIdentifier.ToDescription()));
    }
    public void SetTaskSuccessNotification()
    {
        TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
    }
}
