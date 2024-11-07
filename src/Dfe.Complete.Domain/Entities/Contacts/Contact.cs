using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities.Projects;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Contacts;

public class Contact : IEntity<ContactId>
{
    public ContactId Id { get; set; }

    public ProjectId? ProjectId { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int Category { get; set; }

    public string OrganisationName { get; set; }

    public string Type { get; set; }

    public Guid? LocalAuthorityId { get; set; }

    public int? EstablishmentUrn { get; set; }

    public virtual Project Project { get; set; }
}