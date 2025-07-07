using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record UpdateNoteCommand(NoteId NoteId, string Body) : IRequest<Result<NoteId>>;

public class UpdateNoteCommandHandler(
    INoteWriteRepository _noteWriteRepo,
    INoteReadRepository _noteReadRepo,
    ILogger<UpdateNoteCommandHandler> logger
) : IRequestHandler<UpdateNoteCommand, Result<NoteId>>
{
    public async Task<Result<NoteId>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = new NoteIdQuery(request.NoteId)
                .Apply(_noteReadRepo.Notes())
                .FirstOrDefault();

            if (note is null) return Result<NoteId>.Failure($"Note with ID {request.NoteId.Value} not found", ErrorType.NotFound);

            note.Body = request.Body;
            await _noteWriteRepo.UpdateNoteAsync(note, cancellationToken);

            return Result<NoteId>.Success(note.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateNoteCommandHandler), request);
            return Result<NoteId>.Failure($"Could not update note {request.NoteId.Value}: {ex.Message}");
        }
    }
}