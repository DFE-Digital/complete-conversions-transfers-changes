using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum ProjectType
    {
        [Description("Conversion::Project")]
        Conversion = 1,
        [Description("Transfer::Project")]
        Transfer = 2
    }
}
