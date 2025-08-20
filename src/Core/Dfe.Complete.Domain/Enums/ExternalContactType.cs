using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum ExternalContactType
{
    [Description("HeadTeacher")]
    [DisplayDescription("Headteacher")]
    HeadTeacher = 1,

    [Description("IncomingTrustCEO")]
    [DisplayDescription("Incoming trust CEO (Chief executive officer)")]
    IncomingTrustCEO = 2,

    [Description("OutgoingTrustCEO")]
    [DisplayDescription("Outgoing trust CEO (Chief executive officer)")]
    OutgoingTrustCEO = 3,

    [Description("ChairOfGovernors")]
    [DisplayDescription("Chair of governors")]
    ChairOfGovernors = 4,

    [Description("LocalAuthority")]
    [DisplayDescription("Local authority")]
    LocalAuthority = 5,

    [Description("Solicitor")]
    [DisplayDescription("Solicitor")]
    Solicitor = 6,

    [Description("Diocese")]
    [DisplayDescription("Diocese")]
    Diocese = 7,

    [Description("Other")]
    [DisplayDescription("Someone else")]
    Other = 8,
}


