using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Notes;

[Authorize(policy: UserPolicyConstants.CanAddNotes)]
public class AddProjectNoteModel(ISender sender, ErrorService errorService, ILogger<AddProjectNoteModel> _logger) : ProjectNotesBaseModel(sender, _logger, NotesNavigation)
{
    [BindProperty(Name = "note-text")]
    [Required]
    [DisplayName("note")]
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
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return Page();
        }

        var newNoteQuery = new CreateNoteCommand(new ProjectId(Guid.Parse(ProjectId)), User.GetUserId(), NoteText);
        var response = await Sender.Send(newNoteQuery);

        if (!response.IsSuccess)
            throw new ApplicationException($"An error occurred when creating a new note for project {ProjectId}");

        TempData.SetNotification(
            NotificationType.Success,
            "Success",
            "Your note has been added"
        );

        return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
    }
}
