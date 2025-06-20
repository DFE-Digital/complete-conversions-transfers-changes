using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

[Authorize(policy: UserPolicyConstants.CanAddNotes)]
public class AddProjectNoteModel(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public required string ProjectId { get; set; }

    [BindProperty(Name = "note-text")]
    public required string NoteText { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetProjectByIdQuery(new ProjectId(Guid.Parse(ProjectId)));
        var project = await sender.Send(query);
        if (!project.IsSuccess || project.Value == null)
        {
            return NotFound($"Project {ProjectId} does not exist.");
        }

        if (!project.Value.CanAddNotes)
        {
            TempData.SetNotification(
                NotificationType.Error,
                "Important",
                "The project is not active and no further notes can be added."
            );

            return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var newNote = new CreateNoteCommand(new ProjectId(Guid.Parse(ProjectId)), User.GetUserId(), NoteText);
        var response = await sender.Send(newNote);

        if (!response.IsSuccess)
            throw new ApplicationException($"An error occurred when creating a new note for project {ProjectId}");

        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}

/* TODO 

Refactor this to have a common base class for the project notes pages.
In it we want a notification handler, particularly unauthorised...
It'll probably extend the project navigation model, so we can have the project details

*/