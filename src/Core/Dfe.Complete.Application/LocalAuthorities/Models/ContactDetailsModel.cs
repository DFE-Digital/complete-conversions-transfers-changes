using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.LocalAuthorities.Models
{
    public class ContactDetailsModel
    {
        public ContactId Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Phone { get; set; } = null!;

        public static ContactDetailsModel? MapContactEntityToModel(Contact? contact) => contact == null ? null : new ContactDetailsModel
        {
            Id = contact.Id,
            Title = contact.Title,
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone
        };
    }
}
