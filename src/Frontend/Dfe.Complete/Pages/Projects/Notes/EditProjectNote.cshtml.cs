using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class EditProjectNoteModel(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public required string ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "noteId")]
    public required Guid NoteId { get; set; }

    [BindProperty(Name = "note-text")]
    public required string NoteText { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (NoteId == Guid.Empty)
        {
            return NotFound();
        }

        var noteResult = await sender.Send(new GetNoteByIdQuery(new NoteId(NoteId)));
        if (!noteResult.IsSuccess || noteResult.Value == null)
        {
            return NotFound();
        }

        NoteText = noteResult.Value.Body;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await sender.Send(new UpdateNoteCommand(new NoteId(NoteId), NoteText));
        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}

