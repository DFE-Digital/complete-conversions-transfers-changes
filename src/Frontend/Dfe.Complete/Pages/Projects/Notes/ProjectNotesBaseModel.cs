using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Notes;

public class ProjectNotesBaseModel(ISender sender, string notesNavigation) : ProjectLayoutModel(sender, notesNavigation)
{
    public NoteDto? GetNoteById(Guid noteId)
    {
        if (noteId == Guid.Empty)
            return null;

        var noteResult = sender.Send(new GetNoteByIdQuery(new NoteId(noteId))).GetAwaiter().GetResult();
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

}