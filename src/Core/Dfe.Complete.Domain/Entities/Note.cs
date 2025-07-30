using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class Note : BaseAggregateRoot, IEntity<NoteId>
{
    public NoteId Id { get; set; } = default!;

    public string Body { get; set; } = default!;

    public ProjectId ProjectId { get; set; } = default!;

    public UserId UserId { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? TaskIdentifier { get; set; }

    public Guid? NotableId { get; set; }

    public string? NotableType { get; set; }

    public virtual Project Project { get; set; } = default!;

    public virtual User User { get; set; } = default!;
}
