using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class EditProjectNoteModel(ISender sender) : ProjectNotesBaseModel(sender, NotesNavigation)
{
    [BindProperty(SupportsGet = true, Name = "noteId")]
    public required Guid NoteId { get; set; }

    [BindProperty(Name = "note-text")]
    public required string NoteText { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        var note = GetNoteById(NoteId);
        if (note == null)
            return NotFound();

        if (!CanEditNote(note.UserId))
        {
            TempData.SetNotification(
                NotificationType.Error,
                "Important",
                "You are not authorised to perform this action."
            );
            return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
        }

        NoteText = note.Body;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var response = await Sender.Send(new UpdateNoteCommand(new NoteId(NoteId), NoteText));

        if (!response.IsSuccess)
            throw new ApplicationException($"An error occurred when updating note {NoteId} for project {ProjectId}");

        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}

