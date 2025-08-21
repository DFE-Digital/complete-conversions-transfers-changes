using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Dfe.Complete.Domain.Enums;

public enum ExternalContactType
{
    [Description("headteacher")]
    [DisplayDescription("Headteacher")]
    HeadTeacher = 1,

    [Description("incomingtrustceo")]
    [DisplayDescription("Incoming trust CEO (Chief executive officer)")]   
    IncomingTrustCEO = 2,

    [Description("outgoingtrustceo")]
    [DisplayDescription("Outgoing trust CEO (Chief executive officer)")]
    OutgoingTrustCEO = 3,

    [Description("chairofgovernors")]
    [DisplayDescription("Chair of governors")]
    ChairOfGovernors = 4,

    [Description("localauthority")]
    [DisplayDescription("Local authority")]
    LocalAuthority = 5,

    [Description("solicitor")]
    [DisplayDescription("Solicitor")]    
    Solicitor = 6,

    [Description("diocese")]
    [DisplayDescription("Diocese")]
    Diocese = 7,

    [Description("other")]
    [DisplayDescription("Someone else")]
    Other = 8,
}


