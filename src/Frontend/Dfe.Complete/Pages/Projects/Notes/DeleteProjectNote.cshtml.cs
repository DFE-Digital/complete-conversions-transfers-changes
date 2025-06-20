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

    public async Task<IActionResult> OnPostAsync()
    {
        var response = await sender.Send(new RemoveNoteCommand(new NoteId(NoteId)));

        if (!(response.IsSuccess || response.Value == true))
            throw new ApplicationException($"An error occurred when deleting note {NoteId} for project {ProjectId}");

        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
        // TODO add deleted notification
        // TempData.SetNotification(
    }
}
