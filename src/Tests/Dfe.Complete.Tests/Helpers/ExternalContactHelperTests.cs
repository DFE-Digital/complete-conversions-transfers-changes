using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Helpers;

namespace Dfe.Complete.Tests.Helpers
{
    public class ExternalContactHelperTests
    {
        [Theory]
        [InlineData(ExternalContactType.IncomingTrustCEO, "CEO")]
        [InlineData(ExternalContactType.OutgoingTrustCEO, "CEO")]
        [InlineData(ExternalContactType.HeadTeacher, "Headteacher")]
        [InlineData(ExternalContactType.ChairOfGovernors, "Chair of governors")]
        [InlineData(ExternalContactType.LocalAuthority, "Local authority")]
        [InlineData(ExternalContactType.Solicitor, "Solicitor")]
        [InlineData(ExternalContactType.Diocese, "Diocese")]
        [InlineData(ExternalContactType.Other, "Someone else")]
        public void GetRoleByContactTypeTests(ExternalContactType  value, string expectedResult)
        {
            // Arrange

            // Act
            var result = ExternalContactHelper.GetRoleByContactType(value);

            // Assert
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData(ExternalContactType.IncomingTrustCEO, ContactCategory.IncomingTrust)]
        [InlineData(ExternalContactType.OutgoingTrustCEO, ContactCategory.OutgoingTrust)]
        [InlineData(ExternalContactType.HeadTeacher, ContactCategory.SchoolOrAcademy)]
        [InlineData(ExternalContactType.ChairOfGovernors, ContactCategory.SchoolOrAcademy)]
        [InlineData(ExternalContactType.SchoolOrAcademy, ContactCategory.SchoolOrAcademy)]
        [InlineData(ExternalContactType.LocalAuthority, ContactCategory.LocalAuthority)]
        [InlineData(ExternalContactType.Solicitor, ContactCategory.Solicitor)]
        [InlineData(ExternalContactType.Diocese, ContactCategory.Diocese)]
        [InlineData(ExternalContactType.Other, ContactCategory.Other)]
        public void GetCategoryByContactTypeTests(ExternalContactType value, ContactCategory expectedResult)
        {
            // Arrange

            // Act
            var result = ExternalContactHelper.GetCategoryByContactType(value);

            // Assert
            Assert.Equal(result, expectedResult);
        }
    }
}
