namespace Dfe.Complete.Configuration
{
    public class DataProtectionOptions
    {
        public const string ConfigurationSection = "DataProtection";
        public string? KeyVaultKey { get; init; }
        public string? DpTargetPath { get; init; }
    }
}
