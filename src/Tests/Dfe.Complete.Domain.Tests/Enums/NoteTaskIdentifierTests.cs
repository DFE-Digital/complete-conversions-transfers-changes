using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{ 
    public class NoteTaskIdentifierTests
    {
        [Theory]
        [InlineData(NoteTaskIdentifier.Handover, "handover", "Handover with regional delivery officer")]
        [InlineData(NoteTaskIdentifier.LandQuestionnaire, "land_questionnaire", "Land questionnaire")]
        [InlineData(NoteTaskIdentifier.LandRegistryTitlePlans, "land_registry", "Land registry title plans")]
        [InlineData(NoteTaskIdentifier.StakeholderKickoff, "stakeholder_kick_off", "External stakeholder kick-off")]
        [InlineData(NoteTaskIdentifier.SupplementalFundingAgreement, "supplemental_funding_agreement", "Supplemental funding agreement")]
        [InlineData(NoteTaskIdentifier.ArticleOfAssociation, "articles_of_association", "Articles of association")]
        [InlineData(NoteTaskIdentifier.DeedOfNovationAndVariation, "deed_of_novation_and_variation", "Deed of novation and variation")]
        [InlineData(NoteTaskIdentifier.DeedOfVariation, "deed_of_variation", "Deed of variation")]
        [InlineData(NoteTaskIdentifier.RedactAndSendDocuments, "redact_and_send_documents", "Redact and send documents")]
        [InlineData(NoteTaskIdentifier.ConfirmProposedCapacityOfTheAcademy, "proposed_capacity_of_the_academy", "Confirm the proposed capacity of the academy")]
        [InlineData(NoteTaskIdentifier.ConfirmTransferHasAuthorityToProceed, "conditions_met", "Confirm this transfer has authority to proceed")]   
        [InlineData(NoteTaskIdentifier.DeclarationOfExpenditureCertificate, "declaration_of_expenditure_certificate", "Receive declaration of expenditure certificate")]
        [InlineData(NoteTaskIdentifier.ConfirmAllConditionsMet, "conditions_met", "Confirm all conditions have been met")]
        [InlineData(NoteTaskIdentifier.ReceiveGrantPaymentCertificate, "receive_grant_payment_certificate", "Receive declaration of expenditure certificate")]
        [InlineData(NoteTaskIdentifier.ConfirmAcademyOpenedDate, "confirm_date_academy_opened", "Confirm the date the academy opened")]
        [InlineData(NoteTaskIdentifier.ChurchSupplementalAgreement, "church_supplemental_agreement", "Church supplemental agreement")]        
        [InlineData(NoteTaskIdentifier.CommercialTransferAgreement, "commercial_transfer_agreement", "Commercial transfer agreement")]
        [InlineData(NoteTaskIdentifier.MainContact, "main_contact", "Confirm the main contact")]
        [InlineData(NoteTaskIdentifier.MasterFundingAgreement, "master_funding_agreement", "Master funding agreement")]
        [InlineData(NoteTaskIdentifier.ConfirmIncomingTrustCeoContact, "confirm_incoming_trust_ceo_contact", "Confirm the incoming trust ceo contact")]
        [InlineData(NoteTaskIdentifier.ConfirmRiskProtectionArrangements, "risk_protection_arrangement", "Confirm the academy's risk protection arrangements")]
        [InlineData(NoteTaskIdentifier.ConfirmRiskProtectionArrangementsPolicy, "rpa_policy", "Confirm the academy's risk protection arrangements")]
        [InlineData(NoteTaskIdentifier.CheckAccuracyOfHigherNeeds, "check_accuracy_of_higher_needs", "Check accuracy of high needs places information")]
        [InlineData(NoteTaskIdentifier.CompleteNotificationOfChange, "complete_notification_of_change", "Complete a notification of changes to funded high needs places form")]
        [InlineData(NoteTaskIdentifier.ProcessConversionSupportGrant, "conversion_grant", "Process conversion support grant")]
        [InlineData(NoteTaskIdentifier.ConfirmAndProcessSponsoredSupportGrant, "sponsored_support_grant", "Confirm and process the sponsored support grant")]
        [InlineData(NoteTaskIdentifier.ConfirmHeadTeacherDetails, "confirm_headteacher_contact", "Confirm the headteacher’s details")]
        [InlineData(NoteTaskIdentifier.ConfirmChairOfGovernorsDetails, "confirm_chair_of_governors_contact", "Confirm the chair of governors’ details")]
        [InlineData(NoteTaskIdentifier.TrustModificationOrder, "trust_modification_order", "Trust modification order")]
        [InlineData(NoteTaskIdentifier.DirectionToTransfer, "direction_to_transfer", "Direction to transfer")]
        [InlineData(NoteTaskIdentifier.OneHundredAndTwentyFiveYearLease, "one_hundred_and_twenty_five_year_lease", "125 year lease")]
        [InlineData(NoteTaskIdentifier.Subleases, "subleases", "Subleases")]
        [InlineData(NoteTaskIdentifier.TenancyAtWill, "tenancy_at_will", "Tenancy at will")]
        [InlineData(NoteTaskIdentifier.ConfirmSchoolHasCompletedAllActions, "school_completed", "Confirm the school has completed all actions")]
        [InlineData(NoteTaskIdentifier.ShareInformationAboutOpening, "share_information", "Share the information about opening")]
        [InlineData(NoteTaskIdentifier.ConfirmOutgoingTrustCeoDetails, "confirm_outgoing_trust_ceo_contact", "Confirm the outgoing trust CEO’s details")]
        [InlineData(NoteTaskIdentifier.RequestNewUrnAndRecordForAcademy, "request_new_urn_and_record", "Request a new URN and record for the academy")]
        [InlineData(NoteTaskIdentifier.ConfirmTransferGrantFundingLevel, "sponsored_support_grant", "Confirm transfer grant funding level")]
        [InlineData(NoteTaskIdentifier.CheckAndConfirmAcademyAndTrustFinancialInformation, "check_and_confirm_financial_information", "Check and confirm academy and trust financial information")]
        [InlineData(NoteTaskIdentifier.FormM, "form_m", "Form M")]
        [InlineData(NoteTaskIdentifier.ClosureOrTransferDeclaration, "closure_or_transfer_declaration", "Closure or transfer declaration")]
        [InlineData(NoteTaskIdentifier.ConfirmBankDetailsForGeneralAnnualGrantPaymentNeedToChange, "bank_details_changing", "Confirm if the bank details for the general annual grant payment need to change")]
        [InlineData(NoteTaskIdentifier.ConfirmIncomingTrustHasCompletedAllActions, "confirm_incoming_trust_has_completed_all_actions", "Confirm the incoming trust has completed all actions")]
        public void NoteTaskIdentifier_ShouldHaveCorrectDescription(NoteTaskIdentifier identifier, string expectedDescription, string expectedDisplayExpectation)
        {
            // Act
            var description = identifier.ToDescription();
            var displayDescription = identifier.ToDisplayDescription();

            // Assert
            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedDisplayExpectation, displayDescription);
        } 
    } 
}

