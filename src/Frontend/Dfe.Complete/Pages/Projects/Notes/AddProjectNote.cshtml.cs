using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.Notes;

[Authorize(policy: UserPolicyConstants.CanAddNotes)]
public class AddProjectNoteModel(ISender sender, IErrorService errorService, ILogger<AddProjectNoteModel> logger) : BaseProjectNotesModel(sender, logger, NotesNavigation)
{
    [BindProperty(Name = "note-text")]
    [Required]
    [DisplayName("note")]
    public required string NoteText { get; set; }

    [BindProperty(SupportsGet = true, Name = "task_identifier")]
    public string? TaskIdentifier { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (TaskIdentifier != null)
        {
            var validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);
            if (validTaskIdentifier == null)
                return NotFound();
        }

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

        NoteTaskIdentifier? noteTaskIdentifier = null;
        if (TaskIdentifier != null)
            noteTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);

        var newNoteQuery = new CreateNoteCommand(
            new ProjectId(Guid.Parse(ProjectId)),
            User.GetUserId(),
            NoteText,
            noteTaskIdentifier
        );

        var response = await Sender.Send(newNoteQuery);

        if (!response.IsSuccess)
            throw new InvalidOperationException($"An error occurred when creating a new note for project {ProjectId}");

        TempData.SetNotification(
            NotificationType.Success,
            "Success",
            "Your note has been added"
        );

        return Redirect(GetReturnUrl(TaskIdentifier));
    }
}
