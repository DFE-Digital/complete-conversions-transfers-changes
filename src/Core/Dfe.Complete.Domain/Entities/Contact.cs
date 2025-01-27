using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class Contact : IEntity<ContactId>
{
    public ContactId Id { get; set; }

    public ProjectId? ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ContactCategory Category { get; set; }

    public string? OrganisationName { get; set; }

    public string? Type { get; set; }

    public LocalAuthorityId? LocalAuthorityId { get; set; }

    public int? EstablishmentUrn { get; set; }

    public virtual Project? Project { get; set; }
}
