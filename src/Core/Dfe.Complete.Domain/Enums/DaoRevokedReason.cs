using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum DaoRevokedReason
    {
        [Description("school_rated_good_or_outstanding")]
        [DisplayDescription("School rated good or outstanding")]
        SchoolRatedGoodOrOutstanding = 1,

        [Description("safeguarding_concerns_addressed")]
        [DisplayDescription("Safeguarding concerns addressed")]
        SafeguardingConcernsAddressed = 2,

        [Description("school_closed_or_closing")]
        [DisplayDescription("School closed or closing")]
        SchoolClosedOrClosing = 3,

        [Description("change_to_government_policy")]
        [DisplayDescription("Change to government policy")]
        ChangeToGovernmentPolicy = 4,
    }
}
