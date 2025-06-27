using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum SignificantDateReason
{
    [Description("advisory_board_conditions")]
    AdvisoryBoardConditions = 1,
    [Description("buildings")]
    Buildings = 2,
    [Description("correcting_an_error")]
    CorrectingAnError = 3,
    [Description("diocese")]
    Diocese = 4,
    [Description("finance")]
    Finance = 5,
    [Description("governance")]
    Governance = 6,
    [Description("incoming_trust")]
    IncomingTrust = 7,
    [Description("land")]
    Land = 8,
    [Description("legacy_reason")]
    LegacyReason = 9,
    [Description("legal_documents")]
    LegalDocuments = 10,
    [Description("local_authority")]
    LocalAuthority = 11,
    [Description("negative_press")]
    NegativePress = 12,
    [Description("outgoing_trust")]
    OutgoingTrust = 13,
    [Description("pensions")]
    Pensions = 14,
    [Description("progressing_faster_than_expected")]
    ProgressingFasterThanExpected = 15,
    [Description("school")]
    School = 16,
    [Description("stakeholder_kick_off")]
    StakeholderKickOff = 17,
    [Description("tupe")]
    Tupe = 18,
    [Description("union")]
    Union = 19,
    [Description("viability")]
    Viability = 20,
    [Description("voluntary_deferral")]
    VoluntaryDeferral = 21
}