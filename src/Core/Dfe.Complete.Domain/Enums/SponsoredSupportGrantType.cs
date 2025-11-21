using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum SponsoredSupportGrantType
    {
        [Description("standard")]
        [DisplayDescription("Standard transfer grant")]
        StandardTransferGrant = 1,

        [Description("fast_track")]
        [DisplayDescription("Fast track")]
        FastTrack = 2,

        [Description("intermediate")]
        [DisplayDescription("Intermediate")]
        Intermediate = 3,

        [Description("full_sponsored")]
        [DisplayDescription("Full sponsored")]
        FullSponsored = 4,
    }
}