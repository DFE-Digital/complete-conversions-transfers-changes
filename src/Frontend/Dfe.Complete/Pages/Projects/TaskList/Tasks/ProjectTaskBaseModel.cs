using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Microsoft.AspNetCore.Authorization;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks;

public class BaseProjectTaskModel(ISender sender, IAuthorizationService _authorizationService, NoteTaskIdentifier taskIdentifier) : BaseProjectPageModel(sender)
{
    protected ISender Sender { get; } = sender;
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
            throw new ApplicationException($"Could not load notes for project {ProjectId}");

        Notes = notesResult.Value ?? [];

        return Page();
    }

    public async Task<IActionResult> OnPostAddNoteAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        string? errorMessage = null;
        if (!CanAddNotes)
            errorMessage = "The project is not active and no further notes can be added.";
        else if (!(await _authorizationService.AuthorizeAsync(User, UserPolicyConstants.CanAddNotes)).Succeeded)
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

        return Redirect(string.Format(RouteConstants.ProjectAddTaskNote, ProjectId, TaskIdentifier));
    }
}
// TODO we need to retain the previously visited page for cancelling
