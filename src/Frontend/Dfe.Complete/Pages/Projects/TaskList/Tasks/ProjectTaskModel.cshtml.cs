
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Pages.Projects;

public class TaskListModel(ISender sender) : PageModel
{
    private readonly ISender _sender = sender;

    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string TaskIdentifier { get; set; }

    public List<NoteDto> Notes { get; set; } = new();
    public string Title { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Enum.TryParse<NoteTaskIdentifier>(TaskIdentifier, true, out var taskIdentifier))
        {
            return NotFound();
        }

        Title = $"{taskIdentifier}";

        var noteQuery = new GetProjectTaskNotesByProjectIdQuery(new ProjectId(ProjectId), taskIdentifier);
        var response = await _sender.Send(noteQuery);
        Notes = response.IsSuccess ? response.Value : new List<NoteDto>();

        return Page();
    }

    public IActionResult OnPost()
    {
        // Handle form submission here
        return RedirectToPage("Success");
    }
}
