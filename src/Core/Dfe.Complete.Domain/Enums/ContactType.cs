using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum ContactType
    {
        [Description("Contact::Project")]
        Project = 1,
        [Description("Contact::Establishment")]
        Establishment = 2,
        [Description("Contact::DirectorOfChildServices")]
        DirectorOfChildServices = 3
    }
}
