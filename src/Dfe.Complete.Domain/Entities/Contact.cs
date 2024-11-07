namespace Dfe.Complete.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int Category { get; set; }

    public string? OrganisationName { get; set; }

    public string? Type { get; set; }

    public Guid? LocalAuthorityId { get; set; }

    public int? EstablishmentUrn { get; set; }

    public virtual Project? Project { get; set; }
}
