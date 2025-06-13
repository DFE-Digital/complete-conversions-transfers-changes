using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

public class ViewProjectNotesModel(ISender sender) : ProjectLayoutModel(sender, NotesNavigation)
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
}
