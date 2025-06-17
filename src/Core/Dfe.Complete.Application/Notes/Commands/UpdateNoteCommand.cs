using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

public record UpdateNoteCommand(int NoteId, string Body, int CurrentUserId) : IRequest<NoteId>;

public class UpdateNoteCommandHandler(
    INoteWriteRepository _repo,
    ILogger<UpdateNoteCommand> logger
) : IRequestHandler<UpdateNoteCommand, NoteId>
{
    public async Task<NoteId> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = await _repo.GetNoteByIdAsync(request.NoteId, cancellationToken) ?? throw new NotFoundException($"Note with ID {request.NoteId} not found");

            // if (note.UserId != request.CurrentUserId) throw new UnauthorizedAccessException();

            note.Body = request.Body;
            await _repo.UpdateNoteAsync(note, cancellationToken);

            return note.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateNoteCommand), request);
            throw new Exception(ex.Message);
        }
    }
}