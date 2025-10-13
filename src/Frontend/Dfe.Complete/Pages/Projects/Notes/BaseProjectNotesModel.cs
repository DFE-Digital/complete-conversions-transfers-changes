using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Notes;

public class BaseProjectNotesModel(ISender sender, ILogger logger, string notesNavigation) : ProjectLayoutModel(sender, logger, notesNavigation)
{
    public async Task<NoteDto?> GetNoteById(Guid noteId)
    {
        if (noteId == Guid.Empty)
            return null;

        var noteResult = await Sender.Send(new GetNoteByIdQuery(new NoteId(noteId)));
        if (!noteResult.IsSuccess || noteResult.Value == null)
            return null;

        return noteResult.Value;
    }

    public bool CanAddNotes => Project.State != ProjectState.Deleted && Project.State != ProjectState.Completed && Project.State != ProjectState.DaoRevoked;

    public bool CanEditNote(UserId noteUserId)
    {
        if (Project.State == ProjectState.Completed || noteUserId != User.GetUserId())
            return false;
        return true;
    }

    public bool CanDeleteNote(UserId noteUserId, bool? isNotable)
    {
        if (isNotable == true) return false;
        return CanEditNote(noteUserId);
    }

    public string GetReturnUrl(string? taskIdentifier = null)
    {
        if (taskIdentifier != null)
        {
            NoteTaskIdentifier? noteTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(taskIdentifier);
            if (noteTaskIdentifier != null)
                return string.Format(RouteConstants.ProjectTask, ProjectId, noteTaskIdentifier.ToDescription());
        }

        return string.Format(RouteConstants.ProjectViewNotes, ProjectId);
    }
}
