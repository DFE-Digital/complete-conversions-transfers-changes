using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Configuration
{
    [ExcludeFromCodeCoverage]
    public class PersonsApiOptions
    {
        public string? BaseUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TenantId { get; set; }
        public string? Scope { get; set; }
    }
}
