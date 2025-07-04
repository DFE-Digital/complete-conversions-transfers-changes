using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Pages.Projects;

public class ProjectTaskModel(ISender sender, IAuthorizationService _authorizationService) : BaseProjectPageModel(sender)
{
    public string TaskIdentifier { get; set; } = string.Empty;
    public List<NoteDto> Notes { get; set; } = [];
    public required string Title { get; set; }
    public string SchoolName { get; set; } = string.Empty;

    public string GetReturnUrl() => string.Format(RouteConstants.ProjectTaskList, ProjectId);

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

        var noteQuery = new GetProjectTaskNotesByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId.ToString())), (NoteTaskIdentifier)validTaskIdentifier!);
        var response = await Sender.Send(noteQuery);
        Notes = response.IsSuccess && response.Value != null ? response.Value : [];

        return Page();
    }

    public bool CanAddNotes => Project.State != ProjectState.Deleted && Project.State != ProjectState.Completed && Project.State != ProjectState.DaoRevoked;

    public bool CanEditNote(UserId noteUserId)
    {
        if (Project.State == ProjectState.Completed || noteUserId != User.GetUserId())
            return false;
        return true;
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

        return Redirect(string.Format(RouteConstants.ProjectAddNote, ProjectId));
    }
}
