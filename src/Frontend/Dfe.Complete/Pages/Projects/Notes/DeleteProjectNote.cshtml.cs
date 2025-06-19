using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class DeleteProjectNoteModel(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public required string ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "noteId")]
    public required Guid NoteId { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        await sender.Send(new RemoveNoteCommand(new NoteId(NoteId)));
        // TODO need to return note id again and check success
        return RedirectToPage(string.Format(RouteConstants.ProjectEditNote, ProjectId, NoteId));


    }
}
