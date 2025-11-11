using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record UpdateNoteCommand(NoteId NoteId, string Body) : IRequest<Result<NoteDto>>;

public class UpdateNoteCommandHandler(
    INoteWriteRepository _noteWriteRepo,
    INoteReadRepository _noteReadRepo,
    ILogger<UpdateNoteCommandHandler> logger
) : IRequestHandler<UpdateNoteCommand, Result<NoteDto>>
{
    public async Task<Result<NoteDto>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = new NoteIdQuery(request.NoteId)
                .Apply(_noteReadRepo.Notes())
                .FirstOrDefault();

            if (note is null) return Result<NoteDto>.Failure($"Note with ID {request.NoteId.Value} not found", ErrorType.NotFound);

            note.Body = request.Body;
            await _noteWriteRepo.UpdateNoteAsync(note, cancellationToken);

            var noteDto = new NoteDto(
                note.Id,
                note.Body,
                note.ProjectId,
                note.UserId,
                note.User.FullName,
                note.CreatedAt,
                note.TaskIdentifier,
                note.NotableId != null && note.NotableType != null
            );

            return Result<NoteDto>.Success(noteDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateNoteCommandHandler), request);
            return Result<NoteDto>.Failure($"Could not update note {request.NoteId.Value}: {ex.Message}");
        }
    }
}