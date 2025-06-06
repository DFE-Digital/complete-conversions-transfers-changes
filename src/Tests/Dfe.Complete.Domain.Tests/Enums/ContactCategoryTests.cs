using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Domain.Tests.Enums
{
    public class ContactCategoryTests
    {
        [Theory]
        [InlineData(ContactCategory.SchoolOrAcademy, 1)]
        [InlineData(ContactCategory.IncomingTrust, 2)]
        [InlineData(ContactCategory.OutgoingTrust, 6)]
        [InlineData(ContactCategory.LocalAuthority, 3)]
        [InlineData(ContactCategory.Solicitor, 5)]
        [InlineData(ContactCategory.Diocese, 4)]
        [InlineData(ContactCategory.Other, 0)]
        public void EnumValues_ShouldMatchExpected(ContactCategory category, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)category);
        }
    }
}
