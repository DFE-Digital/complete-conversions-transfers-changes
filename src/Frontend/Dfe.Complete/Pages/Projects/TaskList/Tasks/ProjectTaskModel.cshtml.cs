
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects;

public class TaskListModel(ISender sender) : BaseProjectPageModel(sender)
{
    [BindProperty(SupportsGet = true, Name = "task_identifier")]
    [Required]
    public required string TaskIdentifier { get; set; }
    public List<NoteDto> Notes { get; set; } = new();
    public string Title { get; set; }
    public string SchoolName { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult)
        {
            return baseResult;
        }

        NoteTaskIdentifier? validTaskIdentifier = null;
        if (TaskIdentifier != null)
        {
            validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);
            if (validTaskIdentifier == null)
                return NotFound();
        }

        SchoolName = Establishment?.Name ?? string.Empty;
        Title = $"{validTaskIdentifier}";

        var noteQuery = new GetProjectTaskNotesByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId.ToString())), (NoteTaskIdentifier)validTaskIdentifier!);
        var response = await sender.Send(noteQuery);
        Notes = response.IsSuccess && response.Value != null ? response.Value : [];

        return Page();
    }

    public IActionResult OnPost()
    {
        // Handle form submission here
        return RedirectToPage("Success");
    }
}
