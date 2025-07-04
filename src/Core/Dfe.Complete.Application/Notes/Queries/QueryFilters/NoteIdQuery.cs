using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Notes.Queries.QueryFilters;

public class NoteIdQuery(NoteId? noteId) : IQueryObject<Note>
{
    public IQueryable<Note> Apply(IQueryable<Note> query)
        => noteId == null ? query : query.Where(note => note.Id == noteId);
}
