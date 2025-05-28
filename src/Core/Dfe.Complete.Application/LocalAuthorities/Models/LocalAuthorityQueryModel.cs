using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.LocalAuthorities.Models
{
    public class LocalAuthorityQueryModel
    {
        public required LocalAuthorityId Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
