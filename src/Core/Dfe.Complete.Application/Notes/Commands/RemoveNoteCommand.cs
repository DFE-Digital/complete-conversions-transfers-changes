using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record RemoveNoteCommand(NoteId NoteId) : IRequest;

public class RemoveNoteCommandHandler(
    INoteWriteRepository _repo,
    IHttpContextAccessor _httpContextAccessor,
    ILogger<RemoveNoteCommandHandler> logger
) : IRequestHandler<RemoveNoteCommand>
{
    public async Task Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = await _repo.GetNoteByIdAsync(request.NoteId, cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId.Value} not found");

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                throw new UnauthorizedAccessException();

            if (note.UserId != new UserId(parsedUserId))
                throw new UnauthorizedAccessException("The current user is not assigned to the note and cannot delete it");

            await _repo.RemoveNoteAsync(note, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(RemoveNoteCommandHandler), request);
            throw new Exception(ex.Message);
        }
    }
}