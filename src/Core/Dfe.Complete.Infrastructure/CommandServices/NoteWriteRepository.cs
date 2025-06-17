using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.CommandServices;

internal class NoteWriteRepository : INoteWriteRepository
{
    private readonly CompleteContext _context;
    public NoteWriteRepository(CompleteContext context)
    {
        _context = context;
    }

    public async Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken cancellationToken)
    {
        return await _context.Notes.FindAsync(new object[] { noteId }, cancellationToken);
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync(cancellationToken);
    }
}