using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum AcademyAndTrustFinancialStatus
    {
        [Description("surplus")]
        [DisplayDescription("Surplus")]
        Surplus = 1,

        [Description("deficit")]
        [DisplayDescription("Deficit")]
        Deficit = 2
    }
}