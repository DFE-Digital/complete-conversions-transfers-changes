using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using GovUK.Dfe.CoreLibs.Utilities.Extensions;

namespace Dfe.Complete.Application.Notes.Queries.QueryFilters;

public class ProjectTaskNoteByIdQuery(ProjectId projectId, NoteTaskIdentifier taskIdentifier) : IQueryObject<Note>
{
    public IQueryable<Note> Apply(IQueryable<Note> query)
        => query.Where(note => note.ProjectId == projectId
            && note.TaskIdentifier == taskIdentifier.ToDescription());
}
