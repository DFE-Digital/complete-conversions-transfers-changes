using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities.Projects;
using Dfe.Complete.Domain.Users;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Notes
{
public class Note : IEntity<NoteId>
{
    public NoteId Id { get; set; }

    public string Body { get; set; }

    public ProjectId? ProjectId { get; set; }

    public UserId? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string TaskIdentifier { get; set; }

    public Guid? SignificantDateHistoryId { get; set; }

    public virtual Project Project { get; set; }

    public virtual User User { get; set; }
}
}