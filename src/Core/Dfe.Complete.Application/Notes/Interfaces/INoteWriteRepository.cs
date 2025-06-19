using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Notes.Interfaces;

public interface INoteWriteRepository
{
    Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken cancellationToken);
    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken);
    Task RemoveNoteAsync(Note note, CancellationToken cancellationToken);
}