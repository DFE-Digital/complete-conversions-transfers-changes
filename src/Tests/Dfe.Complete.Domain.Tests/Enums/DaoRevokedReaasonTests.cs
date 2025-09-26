using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{
    public class DaoRevokedReaasonTests
    {
        [Theory]
        [InlineData(DaoRevokedReason.SchoolRatedGoodOrOutstanding, "school_rated_good_or_outstanding", "School rated good or outstanding")]
        [InlineData(DaoRevokedReason.SafeguardingConcernsAddressed, "safeguarding_concerns_addressed", "Safeguarding concerns addressed")]
        [InlineData(DaoRevokedReason.SchoolClosedOrClosing, "school_closed_or_closing", "School closed or closing")]
        [InlineData(DaoRevokedReason.ChangeToGovernmentPolicy, "change_to_government_policy", "Change to government policy")]
        public void NoteTaskIdentifier_ShouldHaveCorrectDescription(DaoRevokedReason identifier, string expectedDescription, string expectedDisplayExpectation)
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