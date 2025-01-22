using Dfe.Complete.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;
// Same here missing implementation of IEntity

public class SignificantDateHistory
{
    public SignificantDateHistoryId Id { get; set; }

    public DateOnly? RevisedDate { get; set; }

    public DateOnly? PreviousDate { get; set; }

    public ProjectId? ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserId? UserId { get; set; }
}
