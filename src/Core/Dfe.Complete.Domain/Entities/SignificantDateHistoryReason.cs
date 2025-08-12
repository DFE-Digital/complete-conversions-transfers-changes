using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class SignificantDateHistoryReason : BaseAggregateRoot, IEntity<SignificantDateHistoryReasonId>
{
    public SignificantDateHistoryReasonId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ReasonType { get; set; }

    public SignificantDateHistoryId? SignificantDateHistoryId { get; set; }
}
