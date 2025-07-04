using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetProjectTaskNotesByProjectIdQuery(ProjectId ProjectId, NoteTaskIdentifier TaskIdentifier) : IRequest<Result<List<NoteDto>>>;

public class GetProjectTaskNotesByProjectIdQueryHandler(
    INoteReadRepository noteReadRepository,
    IMapper mapper,
    ILogger<GetProjectTaskNotesByProjectIdQueryHandler> logger
) : IRequestHandler<GetProjectTaskNotesByProjectIdQuery, Result<List<NoteDto>>>
{
    public async Task<Result<List<NoteDto>>> Handle(
        GetProjectTaskNotesByProjectIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notes = new ProjectTaskNoteByProjectIdAndTaskIdentifierQuery(request.ProjectId, request.TaskIdentifier)
                .Apply(noteReadRepository.Notes())
                .OrderByDescending(n => n.CreatedAt);

            var noteDtos = await notes
                .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<NoteDto>>.Success(noteDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectTaskNotesByProjectIdQueryHandler), request);
            return Result<List<NoteDto>>.Failure(ex.Message);
        }
    }
}
