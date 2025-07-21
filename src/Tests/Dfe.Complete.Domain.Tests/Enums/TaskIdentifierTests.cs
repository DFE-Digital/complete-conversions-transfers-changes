using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{ 
    public class TaskIdentifierTests
    {
        [Theory]
        [InlineData(TaskIdentifier.Handover, "handover")]
        [InlineData(TaskIdentifier.StakeholderKickOff, "stakeholder_kick_off")]
        [InlineData(TaskIdentifier.RiskProtectionArrangement, "risk_protection_arrangement")]
        [InlineData(TaskIdentifier.CheckAccuracyOfHigherNeeds, "check_accuracy_of_higher_needs")]
        [InlineData(TaskIdentifier.CompleteNotificationOfChange, "complete_notification_of_change")]
        [InlineData(TaskIdentifier.ConversionGrant, "conversion_grant")]
        [InlineData(TaskIdentifier.SponsoredSupportGrant, "sponsored_support_grant")]
        [InlineData(TaskIdentifier.AcademyDetails, "academy_details")]
        [InlineData(TaskIdentifier.ConfirmHeadteacherContact, "confirm_headteacher_contact")]
        [InlineData(TaskIdentifier.ConfirmChairOfGovernorsContact, "confirm_chair_of_governors_contact")]
        [InlineData(TaskIdentifier.MainContact, "main_contact")]
        [InlineData(TaskIdentifier.ProposedCapacityOfTheAcademy, "proposed_capacity_of_the_academy")]
        public void TaskIdentifier_ShouldHaveCorrectDescription(TaskIdentifier identifier, string expectedDescription)
        {
            // Act
            var description = identifier.ToDescription();

            // Assert
            Assert.Equal(expectedDescription, description);
        }
    } 
}
