using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects;

public class ProjectTaskModel(ISender sender) : BaseProjectPageModel(sender)
{
    public string TaskIdentifier { get; set; } = string.Empty;
    public List<NoteDto> Notes { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("Success"));
    }

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

}
