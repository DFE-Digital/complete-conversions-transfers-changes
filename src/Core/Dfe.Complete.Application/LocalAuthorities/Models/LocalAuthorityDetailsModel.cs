namespace Dfe.Complete.Application.LocalAuthorities.Models
{
    public class LocalAuthorityDetailsModel
    {
        public LocalAuthorityDto LocalAuthority { get; set; } = null!;
        public ContactDetailsModel? Contact { get; set; } = null!;
    }
}
