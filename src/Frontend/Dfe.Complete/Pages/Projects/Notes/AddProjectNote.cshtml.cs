using Dfe.Complete.Application.Notes.Commands;
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
public class AddProjectNoteModel(ISender sender) : ProjectNotesBaseModel(sender, NotesNavigation)
{
    [BindProperty(Name = "note-text")]
    public required string NoteText { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        if (!CanAddNotes)
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
        var response = await Sender.Send(newNote);

        if (!response.IsSuccess)
            throw new ApplicationException($"An error occurred when creating a new note for project {ProjectId}");

        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}
