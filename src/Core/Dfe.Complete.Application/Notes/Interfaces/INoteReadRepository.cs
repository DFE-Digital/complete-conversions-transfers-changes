using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Notes.Interfaces;

public interface INoteReadRepository
{
    IQueryable<NoteDto> GetNotesForProject(ProjectId projectId);
}
