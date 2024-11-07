using System;
using System.Collections.Generic;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Infrastructure.Models;

public partial class Note
{
    public Guid Id { get; set; }

    public string? Body { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? TaskIdentifier { get; set; }

    public Guid? NotableId { get; set; }

    public string? NotableType { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
