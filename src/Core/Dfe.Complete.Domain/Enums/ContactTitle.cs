using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum ContactTitle
    { 
        [Description("Contact::Project")]
        HeadTeacher = 1,
        [Description("CEO")]
        CEO = 2,
        [Description("Director of Children's Services")]
        MainContact = 3,
        [Description("Chair of Governors")]
        ChairOfGovernors = 4,


    }
}
