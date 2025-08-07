using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class DeleteProjectNoteModel(ISender sender, ILogger<DeleteProjectNoteModel> logger) : BaseProjectNotesModel(sender, logger, NotesNavigation)
{
    [BindProperty(SupportsGet = true, Name = "noteId")]
    public required Guid NoteId { get; set; }

    [BindProperty]
    public string? TaskIdentifier { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        var note = await GetNoteById(NoteId);
        if (note == null)
            return NotFound();

        if (!CanDeleteNote(note.UserId, note.IsNotable))
        {
            TempData.SetNotification(
                NotificationType.Error,
                "Important",
                "You are not authorised to perform this action."
            );
            return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
        }

        TaskIdentifier = note.TaskIdentifier;

        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        var response = await Sender.Send(new RemoveNoteCommand(new NoteId(NoteId)));

        if (!(response.IsSuccess || response.Value == true))
            throw new InvalidOperationException($"An error occurred when deleting note {NoteId} for project {ProjectId}");

        TempData.SetNotification(
            NotificationType.Success,
            "Success",
            "Your note has been deleted"
        );

        return Redirect(GetReturnUrl(TaskIdentifier));
    }
}
