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
        [InlineData(NoteTaskIdentifier.ConfirmIncomingTrustCeoContact, "confirm_incoming_trust_ceo_contact", "Confirm the incoming trust ceo contact")]
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

