using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Helpers;

namespace Dfe.Complete.Tests.Mappers
{
    public class ExternalContactMapperTests
    {
        [Theory]
        [InlineData(ExternalContactType.IncomingTrust, "CEO")]
        [InlineData(ExternalContactType.OutgoingTrust, "CEO")]
        [InlineData(ExternalContactType.HeadTeacher, "Headteacher")]
        [InlineData(ExternalContactType.ChairOfGovernors, "Chair of governors")]
        [InlineData(ExternalContactType.LocalAuthority, "Local authority")]
        [InlineData(ExternalContactType.Solicitor, "Solicitor")]
        [InlineData(ExternalContactType.Diocese, "Diocese")]
        [InlineData(ExternalContactType.Other, "Other")]        
        public void GetRoleByContactTypeTests(ExternalContactType  value, string expectedResult)
        {
            // Act
            var result = ExternalContactMapper.GetRoleByContactType(value);

            // Assert
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData(ExternalContactType.IncomingTrust, ContactCategory.IncomingTrust)]
        [InlineData(ExternalContactType.OutgoingTrust, ContactCategory.OutgoingTrust)]
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
            var result = ExternalContactMapper.MapContactTypeToCategory(value);

            // Assert
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData(ContactCategory.IncomingTrust, ExternalContactType.IncomingTrust)]
        [InlineData(ContactCategory.OutgoingTrust, ExternalContactType.OutgoingTrust)]
        [InlineData(ContactCategory.SchoolOrAcademy, ExternalContactType.SchoolOrAcademy)]        
        [InlineData(ContactCategory.LocalAuthority, ExternalContactType.LocalAuthority)]
        [InlineData(ContactCategory.Solicitor, ExternalContactType.Solicitor)]
        [InlineData(ContactCategory.Diocese, ExternalContactType.Diocese)]
        [InlineData(ContactCategory.Other, ExternalContactType.Other)]
        public void GetContactByCategoryTests(ContactCategory value, ExternalContactType expectedResult)
        {
            // Arrange

            // Act
            var result = ExternalContactMapper.MapCategoryToContactType(value);

            // Assert
            Assert.Equal(result, expectedResult);
        }
    }
}
