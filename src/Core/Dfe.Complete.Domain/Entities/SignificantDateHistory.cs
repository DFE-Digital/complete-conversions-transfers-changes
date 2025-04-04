using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class SignificantDateHistory: IEntity<SignificantDateHistoryId>
{
    public SignificantDateHistoryId Id { get; set; }

    public DateOnly? RevisedDate { get; set; }

    public DateOnly? PreviousDate { get; set; }

    public ProjectId? ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserId? UserId { get; set; }
    
    public virtual Project? Project { get; set; }
}
