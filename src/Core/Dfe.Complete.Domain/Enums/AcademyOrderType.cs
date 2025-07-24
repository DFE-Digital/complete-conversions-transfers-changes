using Dfe.Complete.Utils.Attributes;
using System.ComponentModel; 

namespace Dfe.Complete.Domain.Enums
{
    public enum AcademyOrderType
    {
        [DisplayDescription("AO (Academy order)")]
        AcademyOrder = 1,
        [DisplayDescription("DAO (Directive academy order)")]
        DirectiveAcademyOrder = 2
    }
}
