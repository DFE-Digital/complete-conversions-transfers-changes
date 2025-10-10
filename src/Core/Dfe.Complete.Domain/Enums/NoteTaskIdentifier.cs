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
    LandRegistryTitlePlans = 3,

    [Description("stakeholder_kick_off")]
    [DisplayDescription("External stakeholder kick-off")]
    StakeholderKickoff = 4,

    [Description("supplemental_funding_agreement")]
    [DisplayDescription("Supplemental funding agreement")]
    SupplementalFundingAgreement = 5,

    [Description("articles_of_association")]
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
    [DisplayDescription("Confirm the proposed capacity of the academy")]
    ConfirmProposedCapacityOfTheAcademy = 10,

    [Description("declaration_of_expenditure_certificate")]
    [DisplayDescription("Receive declaration of expenditure certificate")]
    DeclarationOfExpenditureCertificate = 11,

    [Description("conditions_met")]
    [DisplayDescription("Confirm this transfer has authority to proceed")]
    ConfirmTransferHasAuthorityToProceed = 12,

    [Description("confirm_date_academy_transferred")]
    [DisplayDescription("Confirm the date the academy transferred")]
    ConfirmDateAcademyTransferred = 13,

    [Description("conditions_met")]
    [DisplayDescription("Confirm all conditions have been met")]
    ConfirmAllConditionsMet = 14,

    [Description("receive_grant_payment_certificate")]
    [DisplayDescription("Receive declaration of expenditure certificate")]
    ReceiveGrantPaymentCertificate = 15,

    [Description("confirm_date_academy_opened")]
    [DisplayDescription("Confirm the date the academy opened")]
    ConfirmAcademyOpenedDate = 16,

    [Description("church_supplemental_agreement")]
    [DisplayDescription("Church supplemental agreement")]
    ChurchSupplementalAgreement = 17,    

    [Description("commercial_transfer_agreement")]
    [DisplayDescription("Commercial transfer agreement")]
    CommercialTransferAgreement = 18,

    [Description("main_contact")]
    [DisplayDescription("Confirm the main contact")]
    MainContact = 19,

    [Description("check_accuracy_of_higher_needs")]
    [DisplayDescription("Check accuracy of high needs places information")]
    CheckAccuracyOfHigherNeeds = 22,

    [Description("complete_notification_of_change")]
    [DisplayDescription("Complete a notification of changes to funded high needs places form")]
    CompleteNotificationOfChange = 23,

    [Description("conversion_grant")]
    [DisplayDescription("Process conversion support grant")]
    ProcessConversionSupportGrant = 24,

    [Description("sponsored_support_grant")]
    [DisplayDescription("Confirm and process the sponsored support grant")]
    ConfirmAndProcessSponsoredSupportGrant = 25,

    [Description("confirm_headteacher_contact")]
    [DisplayDescription("Confirm the headteacher’s details")]
    ConfirmHeadTeacherDetails = 26,


    [Description("confirm_chair_of_governors_contact")]
    [DisplayDescription("Confirm the chair of governors’ details")]
    ConfirmChairOfGovernorsDetails = 27,

    [Description("trust_modification_order")]
    [DisplayDescription("Trust modification order")]
    TrustModificationOrder = 28,

    [Description("direction_to_transfer")]
    [DisplayDescription("Direction to transfer")]
    DirectionToTransfer  = 29,

    [Description("one_hundred_and_twenty_five_year_lease")]
    [DisplayDescription("125 year lease")]
    OneHundredAndTwentyFiveYearLease = 30,

    [Description("subleases")]
    [DisplayDescription("Subleases")]
    Subleases = 31,

    [Description("tenancy_at_will")]
    [DisplayDescription("Tenancy at will")]
    TenancyAtWill = 32,

    [Description("school_completed")]
    [DisplayDescription("Confirm the school has completed all actions")]
    ConfirmSchoolHasCompletedAllActions = 33,

    [Description("share_information")]
    [DisplayDescription("Share the information about opening")]
    ShareInformationAboutOpening = 34,

    [Description("confirm_outgoing_trust_ceo_contact")]
    [DisplayDescription("Confirm the outgoing trust CEO’s details")]
    ConfirmOutgoingTrustCeoDetails = 35,

    [Description("request_new_urn_and_record")]
    [DisplayDescription("Request a new URN and record for the academy")]
    RequestNewUrnAndRecordForAcademy = 36,

    [Description("sponsored_support_grant")]
    [DisplayDescription("Confirm transfer grant funding level")]
    ConfirmTransferGrantFundingLevel = 37,

    [Description("check_and_confirm_financial_information")]
    [DisplayDescription("Check and confirm academy and trust financial information")]
    CheckAndConfirmAcademyAndTrustFinancialInformation = 38,

    [Description("form_m")]
    [DisplayDescription("Form M")]
    FormM = 39,

    [Description("closure_or_transfer_declaration")]
    [DisplayDescription("Closure or transfer declaration")]
    ClosureOrTransferDeclaration = 40,

    [Description("bank_details_changing")]
    [DisplayDescription("Confirm if the bank details for the general annual grant payment need to change")]
    ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChange = 41,

    [Description("confirm_incoming_trust_has_completed_all_actions")]
    [DisplayDescription("Confirm the incoming trust has completed all actions")]
    ConfirmIncomingTrustHasCompletedAllActions = 42,
}