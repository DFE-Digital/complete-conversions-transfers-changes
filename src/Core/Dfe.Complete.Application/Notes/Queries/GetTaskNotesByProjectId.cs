using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetTaskNotesByProjectIdQuery(ProjectId ProjectId, NoteTaskIdentifier TaskIdentifier) : IRequest<Result<List<NoteDto>>>;

public class GetTaskNotesByProjectIdQueryHandler(INoteReadRepository noteReadRepository,
    IMapper mapper,
    ILogger<GetTaskNotesByProjectIdQueryHandler> logger
) : IRequestHandler<GetTaskNotesByProjectIdQuery, Result<List<NoteDto>>>
{
    public async Task<Result<List<NoteDto>>> Handle(
        GetTaskNotesByProjectIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notes = new ProjectTaskNoteByIdQuery(request.ProjectId, request.TaskIdentifier).Apply(noteReadRepository.Notes())
                .OrderByDescending(n => n.CreatedAt);

            var noteDtos = await notes
                .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<NoteDto>>.Success(noteDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetTaskNotesByProjectIdQueryHandler), request);
            return Result<List<NoteDto>>.Failure(ex.Message);
        }
    }
}
