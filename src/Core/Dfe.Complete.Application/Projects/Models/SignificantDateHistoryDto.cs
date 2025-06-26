using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public class SignificantDateHistoryDto
{
    public SignificantDateHistoryId? Id { get; set; }

    public DateOnly? RevisedDate { get; set; }

    public DateOnly? PreviousDate { get; set; }

    public ProjectId? ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserId? UserId { get; set; }
    
    public SignificantDateHistoryReasonDto Reason { get; set; }
    
    public virtual User User { get; set; }
}
