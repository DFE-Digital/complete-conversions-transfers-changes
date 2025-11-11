using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetNotesByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<List<NoteDto>>>;

public class GetNotesByProjectIdQueryHandler(INoteReadRepository noteReadRepository,
    IMapper mapper,
    ILogger<GetNotesByProjectIdQueryHandler> logger
) : IRequestHandler<GetNotesByProjectIdQuery, Result<List<NoteDto>>>
{

    public async Task<Result<List<NoteDto>>> Handle(
        GetNotesByProjectIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notes = new ProjectNoteByIdQuery(request.ProjectId).Apply(noteReadRepository.Notes())
                .OrderByDescending(n => n.CreatedAt);

            var noteDtos = await notes
                .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<NoteDto>>.Success(noteDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetNotesByProjectIdQueryHandler), request);
            return Result<List<NoteDto>>.Failure(ex.Message);
        }
    }
}
