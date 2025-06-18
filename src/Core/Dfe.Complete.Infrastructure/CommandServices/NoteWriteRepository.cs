using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices;

internal class NoteWriteRepository(CompleteContext context) : INoteWriteRepository
{
    private readonly CompleteContext _context = context;

    public async Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken cancellationToken)
    {
        return await _context.Notes.FindAsync([noteId], cancellationToken);
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync(cancellationToken);
    }
}