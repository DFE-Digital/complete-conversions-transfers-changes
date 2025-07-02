
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Extensions;

namespace Dfe.Complete.Pages.Projects;



public class TaskListModel(ISender sender) : PageModel
{
    private readonly ISender _sender = sender;

    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }


    /// <summary>
    /// The task identifier, as a string from the route. Used to match against NoteTaskIdentifier by description.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string TaskIdentifier { get; set; }

    public List<Note> Notes { get; set; } = new();
    public string Title { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        var taskIdentifier = TaskIdentifier.FromDescriptionValue<NoteTaskIdentifier>();
        if (taskIdentifier == null)
        {
            return NotFound();
        }

        Title = taskIdentifier.Value.ToDescription();

        var notesQuery = new Complete.Application.Notes.Queries.GetProjectTaskNotesByProjectIdQuery(ProjectId, taskIdentifier.Value);
        Notes = await _sender.Send(notesQuery);

        return Page();
    }

    public IActionResult OnPost()
    {
        // Handle form submission here
        return RedirectToPage("Success");
    }
}
