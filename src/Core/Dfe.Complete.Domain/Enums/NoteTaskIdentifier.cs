using System.ComponentModel;
using Dfe.Complete.Utils.Attributes;

namespace Dfe.Complete.Domain.Enums;

public enum NoteTaskIdentifier
{
    [Description("handover")]
    [DisplayDescription("Handover with regional delivery officer")]
    Handover,

    [Description("land_questionnaire")]
    [DisplayDescription("Land questionnaire")]
    LandQuestionnaire,

    [Description("land_registry")]
    [DisplayDescription("Land registry title plans")]
    LandRegistry,

    [Description("stakeholder_kick_off")]
    [DisplayDescription("External stakeholder kick-off")]
    StakeholderKickoff,

    [Description("supplemental_funding_agreement")]
    [DisplayDescription("Supplemental funding agreement")]
    SupplementalFundingAgreement
}