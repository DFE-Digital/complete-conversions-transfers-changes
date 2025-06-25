using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record RemoveNoteCommand(NoteId NoteId) : IRequest<Result<bool>>;

public class RemoveNoteCommandHandler(
    INoteWriteRepository _repo,
    IHttpContextAccessor _httpContextAccessor,
    ILogger<RemoveNoteCommandHandler> logger
) : IRequestHandler<RemoveNoteCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                throw new UnauthorizedAccessException($"Could not delete note {request.NoteId.Value}");

            var note = await _repo.GetNoteByIdAsync(request.NoteId, cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId.Value} not found");

            if (note.UserId != new UserId(parsedUserId))
                throw new UnauthorizedAccessException($"Could not delete note {request.NoteId.Value}");

            await _repo.RemoveNoteAsync(note, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Exception for {Name} Request - {@Request}", nameof(RemoveNoteCommandHandler), request);
            return Result<bool>.Failure(ex.Message, ErrorType.Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(RemoveNoteCommandHandler), request);
            return Result<bool>.Failure($"Could not remove note {request.NoteId.Value}: {ex.Message}");
        }
    }
}