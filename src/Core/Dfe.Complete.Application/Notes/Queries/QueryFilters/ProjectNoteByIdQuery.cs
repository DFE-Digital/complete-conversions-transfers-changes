using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Notes.Queries.QueryFilters;

// Use explicit name ProjectNote - we're not just filtering by project ID, but specifically for notes on a project.
public class ProjectNoteByIdQuery(ProjectId? projectId) : IQueryObject<Note>
{
    public IQueryable<Note> Apply(IQueryable<Note> query)
        => projectId == null ? query : query.Where(note => note.ProjectId == projectId
            && note.NotableId == null && note.NotableType == null && note.TaskIdentifier == null);
}
