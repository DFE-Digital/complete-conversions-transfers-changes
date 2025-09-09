using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum ExternalContactType
{
    [Description("headteacher")]
    [DisplayDescription("Headteacher")]
    HeadTeacher = 1,

    [Description("incomingtrust")]
    [DisplayDescription("Incoming trust CEO (Chief executive officer)")]   
    IncomingTrust = 2,

    [Description("outgoingtrust")]
    [DisplayDescription("Outgoing trust CEO (Chief executive officer)")]
    OutgoingTrust = 3,

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

    [Description("schoolacademy")]
    [DisplayDescription("School or academy")]
    SchoolOrAcademy = 8,

    [Description("someoneelse")]
    [DisplayDescription("Someone else")]
    SomeOneElse = 9,

    [Description("other")]
    [DisplayDescription("Other")]
    Other = 10,
}


