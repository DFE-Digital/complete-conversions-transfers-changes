using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class ViewProjectNotesModel(ISender sender, IAuthorizationService _authorizationService) : ProjectNotesBaseModel(sender, NotesNavigation)
{
    public IReadOnlyList<NoteDto> Notes { get; private set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        var notesResult = await Sender.Send(new GetNotesByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId))));
        if (!notesResult.IsSuccess)
        {
            throw new ApplicationException($"Could not load notes for project {ProjectId}");
        }

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

        return Redirect(string.Format(RouteConstants.ProjectAddNote, ProjectId));
    }

}
