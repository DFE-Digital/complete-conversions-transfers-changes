using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class ViewProjectNotesModel(ISender sender) : ProjectLayoutModel(sender, NotesNavigation)
{
    public IReadOnlyList<NoteDto> Notes { get; private set; } = [];

    public async Task<IActionResult> OnPostAddNoteAsync(IAuthorizationService _authorizationService)
    {
        var canAddNotes = Project.State != ProjectState.Deleted &&
            Project.State != ProjectState.Completed &&
            Project.State != ProjectState.DaoRevoked &&
            (await _authorizationService.AuthorizeAsync(User, UserPolicyConstants.CanAddNotes)).Succeeded;

        if (!canAddNotes)
        {
            TempDataExtensions.SetNotification(
                TempData,
                NotificationType.Error,
                "Access denied",
                "You cannot add notes to this project.");

            return Page();
        }

        return RedirectToPage("/Projects/Notes/Add");
    }

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
}
