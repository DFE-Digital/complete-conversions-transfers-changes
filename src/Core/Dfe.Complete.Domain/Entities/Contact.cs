using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class Contact : BaseAggregateRoot, IEntity<ContactId>
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

    public ContactType? Type { get; set; }

    public LocalAuthorityId? LocalAuthorityId { get; set; }

    public int? EstablishmentUrn { get; set; }

    public virtual Project? Project { get; set; }

    public static Contact Create(ContactId id,
        string title,
        string name,
        string? email,
        string? phone,
        LocalAuthorityId localAuthorityId,
        DateTime createdAt
        )
    {
        return new Contact
        {
            Id = id,
            Title = title,
            Name = name,
            Email = email,
            Phone = phone,
            LocalAuthorityId = localAuthorityId,
            Category = ContactCategory.LocalAuthority,
            Type = ContactType.DirectorOfChildServices,
            CreatedAt = createdAt
        };
    }

    public void Update(
        string title,
        string name,
        string? email,
        string? phone,
        DateTime updatedAt)
    {
        Title = title;
        Name = name;
        Email = email;
        Phone = phone;
        UpdatedAt = updatedAt;
    }
}
