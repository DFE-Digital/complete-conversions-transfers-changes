using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum ProjectState
    {
        [Description("Active")]
        Active = 0,
        [Description("Completed")]
        Completed = 1,
        [Description("Deleted")]
        Deleted = 2,
        [Description("Dao Revoked")]
        DaoRevoked = 3,
        [Description("In Active")]
        Inactive = 4
    }
}
