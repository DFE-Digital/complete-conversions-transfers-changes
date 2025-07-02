
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects;

public class TaskListModel(ISender sender) : PageModel
{
    private readonly ISender _sender = sender;

    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "task_identifier")]
    [Required]
    public required string TaskIdentifier { get; set; }
    public List<NoteDto> Notes { get; set; } = new();
    public string Title { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NoteTaskIdentifier? validTaskIdentifier = null;
        if (TaskIdentifier != null)
        {
            validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);
            if (validTaskIdentifier == null)
                return NotFound();
        }

        Title = $"{validTaskIdentifier}";

        var noteQuery = new GetProjectTaskNotesByProjectIdQuery(new ProjectId(ProjectId), (NoteTaskIdentifier)validTaskIdentifier!);
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
