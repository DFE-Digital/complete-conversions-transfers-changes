using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Utilities.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Commands;

public record CreateNoteCommand(ProjectId ProjectId, UserId UserId, string Body, NoteTaskIdentifier? TaskIdentifier = null) : IRequest<Result<NoteId>>;

public class CreateNoteCommandHandler(
    INoteWriteRepository _repo,
    ILogger<CreateNoteCommandHandler> logger
) : IRequestHandler<CreateNoteCommand, Result<NoteId>>
{
    public async Task<Result<NoteId>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var note = new Note()
            {
                Id = new NoteId(Guid.NewGuid()),
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                Body = request.Body,
                TaskIdentifier = request.TaskIdentifier.ToDescription(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.CreateNoteAsync(note, cancellationToken);

            return Result<NoteId>.Success(note.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(CreateNoteCommandHandler), request);
            return Result<NoteId>.Failure($"Could not create note for project {request.ProjectId.Value}: {ex.Message}");
        }
    }
}
