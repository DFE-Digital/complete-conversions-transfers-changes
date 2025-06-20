using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record UpdateNoteCommand(NoteId NoteId, string Body) : IRequest<Result<NoteId>>;

public class UpdateNoteCommandHandler(
    INoteWriteRepository _repo,
    IHttpContextAccessor _httpContextAccessor,
    ILogger<UpdateNoteCommandHandler> logger
) : IRequestHandler<UpdateNoteCommand, Result<NoteId>>
{
    public async Task<Result<NoteId>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = await _repo.GetNoteByIdAsync(request.NoteId, cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId.Value} not found");

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                throw new UnauthorizedAccessException();

            if (note.UserId != new UserId(parsedUserId))
                throw new UnauthorizedAccessException("The current user is not assigned to the note and cannot change it");

            note.Body = request.Body;
            await _repo.UpdateNoteAsync(note, cancellationToken);

            return Result<NoteId>.Success(note.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateNoteCommandHandler), request);
            return Result<NoteId>.Failure($"Could not update note {request.NoteId.Value}: {ex.Message}");
        }
    }
}