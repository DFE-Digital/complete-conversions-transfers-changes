using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class NoteReadRepository(CompleteContext context, IMapper mapper) : INoteReadRepository
{
    public IQueryable<NoteDto> GetNotesForProject(ProjectId projectId) => context.Notes
        .AsNoTracking()
        .Include(n => n.User)
        .Where(n => n.ProjectId == projectId
            && n.NotableId == null && n.NotableType == null && n.TaskIdentifier == null)
        .ProjectTo<NoteDto>(mapper.ConfigurationProvider);
}
