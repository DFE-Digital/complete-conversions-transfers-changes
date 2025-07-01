using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public class SignificantDateHistoryReasonDto
{
    public SignificantDateHistoryReasonId? Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ReasonType { get; set; }

    public SignificantDateHistoryId? SignificantDateHistoryId { get; set; }
}
