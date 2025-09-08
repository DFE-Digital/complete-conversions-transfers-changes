using System.ComponentModel;
using Dfe.Complete.Utils.Attributes;

namespace Dfe.Complete.Domain.Enums;

public enum NoteTaskIdentifier
{
    [Description("handover")]
    [DisplayDescription("Handover with regional delivery officer")]
    Handover = 1,

    [Description("land_questionnaire")]
    [DisplayDescription("Land questionnaire")]
    LandQuestionnaire = 2,

    [Description("land_registry")]
    [DisplayDescription("Land registry title plans")]
    LandRegistry = 3,

    [Description("stakeholder_kick_off")]
    [DisplayDescription("External stakeholder kick-off")]
    StakeholderKickoff = 4,

    [Description("supplemental_funding_agreement")]
    [DisplayDescription("Supplemental funding agreement")]
    SupplementalFundingAgreement = 5,

    [Description("article_of_association")]
    [DisplayDescription("Articles of association")]
    ArticleOfAssociation = 6,

    [Description("deed_of_novation_and_variation")]
    [DisplayDescription("Deed of novation and variation")]
    DeedOfNovationAndVariation = 7,

    [Description("deed_of_variation")]
    [DisplayDescription("Deed of variation")]
    DeedOfVariation = 8,

    [Description("redact_and_send_documents")]
    [DisplayDescription("Redact and send documents")]
    RedactAndSendDocuments = 9,

    [Description("proposed_capacity_of_the_academy")]
    [DisplayDescription("Confirm Proposed Capacity Of TheAcademy")]
    ConfirmProposedCapacityOfTheAcademy = 10,

    [Description("receive_declaration_of_expenditure_certificate")]
    [DisplayDescription("Receive declaration of expenditure certificate")]
    DeclarationOfExpenditureCertificate = 11
}