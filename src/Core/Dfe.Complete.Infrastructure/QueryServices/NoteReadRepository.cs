using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class NoteReadRepository(CompleteContext context) : INoteReadRepository
{
    public IQueryable<Note> Notes() => context.Notes
        .AsNoTracking()
        .Include(n => n.User);
}
