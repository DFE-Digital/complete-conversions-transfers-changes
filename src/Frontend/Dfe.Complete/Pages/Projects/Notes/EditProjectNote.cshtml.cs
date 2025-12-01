using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.Notes;

public class EditProjectNoteModel(ISender sender, IErrorService errorService, ILogger<EditProjectNoteModel> logger, IProjectPermissionService projectPermissionService) : BaseProjectNotesModel(sender, logger, NotesNavigation, projectPermissionService)
{
    [BindProperty(SupportsGet = true, Name = "noteId")]
    public required Guid NoteId { get; set; }

    [BindProperty(Name = "note-text")]
    [Required]
    [DisplayName("note")]
    public required string NoteText { get; set; }

    public string? TaskIdentifier { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        var note = await GetNoteById(NoteId);
        if (note == null)
            return NotFound();

        if (!CanEditNote(note.UserId))
        {
            TempData.SetNotification(
                NotificationType.Error,
                "Important",
                "You are not authorised to perform this action."
            );
            return Redirect(string.Format(RouteConstants.ProjectViewNotes, ProjectId));
        }

        NoteText = note.Body;
        TaskIdentifier = note.TaskIdentifier;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return Page();
        }

        var response = await Sender.Send(new UpdateNoteCommand(new NoteId(NoteId), NoteText));

        if (!response.IsSuccess || response.Value == null)
            throw new InvalidOperationException($"An error occurred when updating note {NoteId} for project {ProjectId}");

        TempData.SetNotification(
            NotificationType.Success,
            "Success",
            "Your note has been edited"
        );

        return Redirect(GetReturnUrl(response.Value.TaskIdentifier));
    }
}

