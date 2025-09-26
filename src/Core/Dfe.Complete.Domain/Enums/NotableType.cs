using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum NotableType
    {
        [Description("SignificantDateHistoryReason")]
        [DisplayDescription("Significant date history reason")]
        SignificantDateHistoryReason = 1,

        [Description("DaoRevocationReason")]
        [DisplayDescription("Dao revocation reason")]
        DaoRevocationReason = 2,
    }
}
