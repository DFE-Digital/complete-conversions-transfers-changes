using Dfe.Complete.Domain.Enums;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Tests.Enums
{
    public class ContactTypeTests
    {
        [Theory]
        [InlineData(ContactType.Project, 1)]
        [InlineData(ContactType.Establishment, 2)]
        [InlineData(ContactType.DirectorOfChildServices, 3)]
        public void EnumValues_ShouldMatchExpected(ContactType contactType, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)contactType);
        }

        [Theory]
        [InlineData(ContactType.Project, "Contact::Project")]
        [InlineData(ContactType.Establishment, "Contact::Establishment")]
        [InlineData(ContactType.DirectorOfChildServices, "Contact::DirectorOfChildServices")]
        public void EnumDescriptions_ShouldMatchExpected(ContactType contactType, string expectedDescription)
        {
            var description = GetEnumDescription(contactType);
            Assert.Equal(expectedDescription, description);
        }

        private static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }

    }
}
