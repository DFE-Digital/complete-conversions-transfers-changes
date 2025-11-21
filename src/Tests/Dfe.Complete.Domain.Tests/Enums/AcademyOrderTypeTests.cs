using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{
    public class AcademyOrderTypeTests
    {
        [Theory]
        [InlineData(AcademyOrderType.AcademyOrder, "AO (Academy order)")]
        [InlineData(AcademyOrderType.DirectiveAcademyOrder, "DAO (Directive academy order)")]
        public void TaskIdentifier_ShouldHaveCorrectDescription(AcademyOrderType academyOrderType, string expectedDescription)
        {
            // Act
            var description = academyOrderType.ToDisplayDescription();

            // Assert
            Assert.Equal(expectedDescription, description);
        }
    }
}
