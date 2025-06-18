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
    public string NoteText { get; set; }

    public async Task OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await sender.Send(new UpdateNoteCommand(new NoteId(NoteId), NoteText));
        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}

