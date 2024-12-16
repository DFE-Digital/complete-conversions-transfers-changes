using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Infrastructure.Models;

public class SignificantDateHistoryReason
{
    public SignificantDateHistoryReasonId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ReasonType { get; set; }

    public SignificantDateHistoryId? SignificantDateHistoryId { get; set; }
}
