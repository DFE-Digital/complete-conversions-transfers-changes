using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Notes.Interfaces;

public interface INoteWriteRepository
{
    Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken cancellationToken);
    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken);
}