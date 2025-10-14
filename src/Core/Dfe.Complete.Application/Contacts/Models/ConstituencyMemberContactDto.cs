using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Contacts.Models
{
    public record ConstituencyMemberContactDto
    {   
        public string DisplayNameWithTitle { get; set; } = null!;        

        public string? Email { get; set; }       
    }
}
