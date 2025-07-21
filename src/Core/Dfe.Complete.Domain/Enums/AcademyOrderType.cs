using System.ComponentModel; 

namespace Dfe.Complete.Domain.Enums
{
    public enum AcademyOrderType
    {
        [Description("AO (Academy order)")]
        AcademyOrder = 0,
        [Description("DAO (Directive academy order)")]
        DirectiveAcademyOrder = 1
    }
}
