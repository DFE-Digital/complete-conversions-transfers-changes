using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record UpdateNoteCommand(NoteId NoteId, string Body) : IRequest<Result<NoteDto>>;

public class UpdateNoteCommandHandler(
    INoteWriteRepository _repo,
    IHttpContextAccessor _httpContextAccessor,
    ILogger<UpdateNoteCommandHandler> logger
) : IRequestHandler<UpdateNoteCommand, Result<NoteDto>>
{
    public async Task<Result<NoteDto>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                throw new UnauthorizedAccessException($"Could not update note {request.NoteId.Value}");

            var note = await _repo.GetNoteByIdAsync(request.NoteId, cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId.Value} not found");

            if (note.UserId != new UserId(parsedUserId))
                throw new UnauthorizedAccessException($"Could not update note {request.NoteId.Value}");

            note.Body = request.Body;
            await _repo.UpdateNoteAsync(note, cancellationToken);

            var noteDto = new NoteDto(
                note.Id,
                note.Body,
                note.ProjectId,
                note.UserId,
                note.User.FullName,
                note.CreatedAt,
                note.TaskIdentifier
            );

            return Result<NoteDto>.Success(noteDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Exception for {Name} Request - {@Request}", nameof(RemoveNoteCommandHandler), request);
            return Result<NoteDto>.Failure(ex.Message, ErrorType.Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateNoteCommandHandler), request);
            return Result<NoteDto>.Failure($"Could not update note {request.NoteId.Value}: {ex.Message}");
        }
    }
}