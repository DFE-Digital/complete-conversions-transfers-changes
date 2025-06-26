using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum NoteTaskIdentifier
{
    [Description("handover")]
    Handover,

    [Description("stakeholder_kick_off")]
    StakeholderKickOff,

    [Description("rpa_policy")]
    RpaPolicy,

    [Description("confirm_headteacher_contact")]
    ConfirmHeadteacherContact,

    [Description("confirm_incoming_trust_ceo_contact")]
    ConfirmIncomingTrustCeoContact,

    [Description("confirm_outgoing_trust_ceo_contact")]
    ConfirmOutgoingTrustCeoContact,

    [Description("main_contact")]
    MainContact,

    [Description("request_new_urn_and_record")]
    RequestNewUrnAndRecord,

    [Description("sponsored_support_grant")]
    SponsoredSupportGrant,

    [Description("check_and_confirm_financial_information")]
    CheckAndConfirmFinancialInformation,

    [Description("form_m")]
    FormM,

    [Description("land_consent_letter")]
    LandConsentLetter,

    [Description("supplemental_funding_agreement")]
    SupplementalFundingAgreement,

    [Description("deed_of_novation_and_variation")]
    DeedOfNovationAndVariation,

    [Description("church_supplemental_agreement")]
    ChurchSupplementalAgreement,

    [Description("master_funding_agreement")]
    MasterFundingAgreement,

    [Description("articles_of_association")]
    ArticlesOfAssociation,

    [Description("deed_of_variation")]
    DeedOfVariation,

    [Description("deed_of_termination_for_the_master_funding_agreement")]
    DeedOfTerminationForTheMasterFundingAgreement,

    [Description("deed_termination_church_agreement")]
    DeedTerminationChurchAgreement,

    [Description("commercial_transfer_agreement")]
    CommercialTransferAgreement,

    [Description("closure_or_transfer_declaration")]
    ClosureOrTransferDeclaration,

    [Description("bank_details_changing")]
    BankDetailsChanging,

    [Description("confirm_incoming_trust_has_completed_all_actions")]
    ConfirmIncomingTrustHasCompletedAllActions,

    [Description("conditions_met")]
    ConditionsMet,

    [Description("confirm_date_academy_transferred")]
    ConfirmDateAcademyTransferred,

    [Description("redact_and_send_documents")]
    RedactAndSendDocuments,

    [Description("declaration_of_expenditure_certificate")]
    DeclarationOfExpenditureCertificate
}