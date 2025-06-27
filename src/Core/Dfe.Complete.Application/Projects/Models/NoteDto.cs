using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public sealed record NoteDto(
    NoteId Id,
    string Body,
    ProjectId ProjectId,
    UserId UserId,
    string UserFullName,
    DateTime CreatedAt
);