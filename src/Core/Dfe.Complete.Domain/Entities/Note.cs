using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class Note : BaseAggregateRoot, IEntity<NoteId>
{
    public NoteId Id { get; set; }

    public string? Body { get; set; }

    public ProjectId? ProjectId { get; set; }

    public UserId? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? TaskIdentifier { get; set; }

    public Guid? NotableId { get; set; }

    public string? NotableType { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
