using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum RiskProtectionArrangementOption
    {
        [Description("standard")]
        Standard = 1,
        [Description("church_or_trust")]
        ChurchOrTrust = 2,
        [Description("commercial")]
        Commercial = 3
    }
}
