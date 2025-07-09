using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Notes.Interfaces;

public interface INoteWriteRepository
{
    Task CreateNoteAsync(Note note, CancellationToken cancellationToken);
    Task RemoveNoteAsync(Note note, CancellationToken cancellationToken);
    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken);
}