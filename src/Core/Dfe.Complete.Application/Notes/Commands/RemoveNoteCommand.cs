using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record RemoveNoteCommand(NoteId NoteId) : IRequest<Result<bool>>;

public class RemoveNoteCommandHandler(
    INoteWriteRepository _noteWriteRepo,
    INoteReadRepository _noteReadRepo,
    ILogger<RemoveNoteCommandHandler> logger
) : IRequestHandler<RemoveNoteCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = new NoteIdQuery(request.NoteId)
                .Apply(_noteReadRepo.Notes())
                .FirstOrDefault();

            if (note is null) return Result<bool>.Failure($"Note with ID {request.NoteId.Value} not found", ErrorType.NotFound);

            await _noteWriteRepo.RemoveNoteAsync(note, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(RemoveNoteCommandHandler), request);
            return Result<bool>.Failure($"Could not remove note {request.NoteId.Value}: {ex.Message}");
        }
    }
}