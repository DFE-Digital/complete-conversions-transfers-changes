using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{ 
    public class NoteTaskIdentifierTests
    {
        [Theory]
        [InlineData(NoteTaskIdentifier.Handover, "handover", "Handover with regional delivery officer")]
        [InlineData(NoteTaskIdentifier.LandQuestionnaire, "land_questionnaire", "Land questionnaire")]
        [InlineData(NoteTaskIdentifier.LandRegistry, "land_registry", "Land registry title plans")]
        [InlineData(NoteTaskIdentifier.StakeholderKickoff, "stakeholder_kick_off", "External stakeholder kick-off")]
        [InlineData(NoteTaskIdentifier.SupplementalFundingAgreement, "supplemental_funding_agreement", "Supplemental funding agreement")]
        [InlineData(NoteTaskIdentifier.ArticleOfAssociation, "article_of_association", "Articles of association")]
        [InlineData(NoteTaskIdentifier.DeedOfNovationAndVariation, "deed_of_novation_and_variation", "Deed of novation and variation")]
        [InlineData(NoteTaskIdentifier.DeedOfVariation, "deed_of_variation", "Deed of variation")]
        [InlineData(NoteTaskIdentifier.RedactAndSendDocuments, "redact_and_send_documents", "Redact and send documents")]
        [InlineData(NoteTaskIdentifier.ConfirmProposedCapacityOfTheAcademy, "proposed_capacity_of_the_academy", "Confirm the proposed capacity of the academy")]
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
