using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Utils.Exceptions;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetNoteByIdQuery(NoteId NoteId) : IRequest<Result<NoteDto>>;

public class GetNoteByIdQueryHandler(INoteReadRepository noteReadRepository,
    IMapper mapper,
    ILogger<GetNoteByIdQueryHandler> logger
) : IRequestHandler<GetNoteByIdQuery, Result<NoteDto>>
{

    public async Task<Result<NoteDto>> Handle(
        GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var note = new NoteIdQuery(request.NoteId)
                .Apply(noteReadRepository.Notes());

            var noteDto = await note
                .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId.Value} not found");
            return Result<NoteDto>.Success(noteDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetNoteByIdQueryHandler), request);
            return Result<NoteDto>.Failure(ex.Message);
        }
    }
}
