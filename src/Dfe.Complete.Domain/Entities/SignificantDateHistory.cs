using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class SignificantDateHistory
{
    public Guid Id { get; set; }

    public DateOnly? RevisedDate { get; set; }

    public DateOnly? PreviousDate { get; set; }

    public Guid? ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? UserId { get; set; }
}
