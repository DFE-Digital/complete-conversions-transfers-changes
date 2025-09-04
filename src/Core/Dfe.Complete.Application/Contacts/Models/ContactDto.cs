using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Contacts.Models
{
    public record ContactDto
    {
        public ContactId Id { get; set; } = default!;

        public ProjectId? ProjectId { get; set; }

        public string Name { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }       

        public ContactCategory Category { get; set; }

        public string? OrganisationName { get; set; }

        public ContactType? Type { get; set; }

        public LocalAuthorityId? LocalAuthorityId { get; set; }

        public int? EstablishmentUrn { get; set; }

    }
}
