using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Application.Contacts.Models
{
    [ExcludeFromCodeCoverage]
    public record ConstituencyMemberContactDto
    {
        public string DisplayNameWithTitle { get; set; } = null!;

        public string? Email { get; set; }
    }
}
