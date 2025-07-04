using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using DfE.CoreLibs.Utilities.Extensions;

namespace Dfe.Complete.Application.Notes.Queries.QueryFilters;

public class ProjectTaskNoteByProjectIdAndTaskIdentifierQuery(ProjectId projectId, NoteTaskIdentifier taskIdentifier)
{
    public IQueryable<Note> Apply(IQueryable<Note> notes) =>
        notes.Where(n => n.ProjectId == projectId && n.TaskIdentifier == taskIdentifier.ToDescription());
}