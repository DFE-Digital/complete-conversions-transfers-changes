using System.ComponentModel;
using Dfe.Complete.Utils.Attributes;

namespace Dfe.Complete.Domain.Enums;

public enum SignificantDateReason
{
    
    [Description("advisory_board_conditions")]
    [DisplayDescription("Advisory board conditions not met")]
    AdvisoryBoardConditions = 1,
    [Description("buildings")]
    [DisplayDescription("Buildings")]
    Buildings = 2,
    [Description("correcting_an_error")]
    [DisplayDescription("Correcting an error")]
    CorrectingAnError = 3,
    [Description("Diocese")]
    [DisplayDescription("Advisory board conditions not met")]
    Diocese = 4,
    [Description("Finance")]
    [DisplayDescription("Advisory board conditions not met")]
    Finance = 5,
    [Description("Governance")]
    [DisplayDescription("Advisory board conditions not met")]
    Governance = 6,
    [Description("incoming_trust")]
    [DisplayDescription("Incoming trust")]
    IncomingTrust = 7,
    [Description("land")]
    [DisplayDescription("Land")]
    Land = 8,
    [Description("legacy_reason")]
    [DisplayDescription("Legacy reason, see note")]
    LegacyReason = 9,
    [Description("legal_documents")]
    [DisplayDescription("Legal Documents")]
    LegalDocuments = 10,
    [Description("local_authority")]
    [DisplayDescription("Local Authority")]
    LocalAuthority = 11,
    [Description("negative_press")]
    [DisplayDescription("Negative press")]
    NegativePress = 12,
    [Description("outgoing_trust")]
    [DisplayDescription("Outgoing trust")]
    OutgoingTrust = 13,
    [Description("pensions")]
    [DisplayDescription("Pensions")]
    Pensions = 14,
    [Description("progressing_faster_than_expected")]
    [DisplayDescription("Project is progressing faster than expected")]
    ProgressingFasterThanExpected = 15,
    [Description("school")]
    [DisplayDescription("School")]
    School = 16,
    [Description("stakeholder_kick_off")]
    [DisplayDescription("Confirmed as part of the stakeholder kick off task")]
    StakeholderKickOff = 17,
    [Description("tupe")]
    [DisplayDescription("TuPE (Transfer of Undertakings Protection Employment rights)")]
    Tupe = 18,
    [Description("union")]
    [DisplayDescription("Union")]
    Union = 19,
    [Description("viability")]
    [DisplayDescription("Viability")]
    Viability = 20,
    [Description("voluntary_deferral")]
    [DisplayDescription("Voluntary deferral")]
    VoluntaryDeferral = 21
}