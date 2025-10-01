using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{
    public class NotableTypeTests
    {
        [Theory]
        [InlineData(NotableType.SignificantDateHistoryReason, "SignificantDateHistoryReason", "Significant date history reason")]
        [InlineData(NotableType.DaoRevocationReason, "DaoRevocationReason", "Dao revocation reason")]
        public void NoteTaskIdentifier_ShouldHaveCorrectDescription(NotableType identifier, string expectedDescription, string expectedDisplayExpectation)
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
