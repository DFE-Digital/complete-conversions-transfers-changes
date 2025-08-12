using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Contacts.Models
{
    public class KeyContactDto
    {
        public KeyContactId? Id { get; set; }
        public ContactId? HeadteacherId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ContactId? ChairOfGovernorsId { get; set; }

        public ContactId? IncomingTrustCeoId { get; set; }

        public ContactId? OutgoingTrustCeoId { get; set; }
    }
}
