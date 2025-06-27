using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Notes.Interfaces;

public interface INoteReadRepository
{
    IQueryable<Note> Notes();
}
