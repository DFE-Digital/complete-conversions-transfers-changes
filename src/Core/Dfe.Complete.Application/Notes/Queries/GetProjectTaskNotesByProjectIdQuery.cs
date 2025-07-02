using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Application.Notes.Queries
{
    public record GetProjectTaskNotesByProjectIdQuery(Guid ProjectId, NoteTaskIdentifier TaskIdentifier) : IRequest<List<Note>>;
}
