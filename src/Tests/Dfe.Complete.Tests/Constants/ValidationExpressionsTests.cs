using Dfe.Complete.Constants;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Tests.Constants
{
    public partial class ValidationExpressionsTests
    {
        [GeneratedRegex(ValidationExpressions.UKPhone)]
        private static partial Regex UKPhoneRegex();
        [GeneratedRegex(ValidationExpressions.UKPostCode, RegexOptions.IgnoreCase, "en-GB")]
        private static partial Regex UKPostCodeRegex();
        [GeneratedRegex(ValidationExpressions.Email)]
        private static partial Regex EmailRegex();

        [Theory]
        [InlineData("SW1A 1AA", true)]
        [InlineData("M1 1AE", true)]
        [InlineData("B33 8TH", true)]
        [InlineData("CR2 6XH", true)]
        [InlineData("DN55 1PT", true)]
        [InlineData("INVALID", false)]
        [InlineData("12345", false)]
        public void UKPostCode_Validation(string input, bool expectedIsValid)
        {
            var isValid = UKPostCodeRegex().IsMatch(input);
            Assert.Equal(expectedIsValid, isValid);
        }

        [Theory]
        [InlineData("+44 7123 456 789", true)]
        [InlineData("+447123456789", true)]
        [InlineData("07123 456789", true)]
        [InlineData("07123456789", true)]
        [InlineData("(07123) 456789", true)]
        [InlineData("+44 1234 567 890", true)]
        [InlineData("01234 567890", true)]
        [InlineData("01234567890", true)]
        [InlineData("INVALID", false)]
        [InlineData("123456", false)]
        [InlineData("020 8327 3737", true)]
        public void UKPhone_Validation(string input, bool expectedIsValid)
        {
            var isValid = UKPhoneRegex().IsMatch(input);
            Assert.Equal(expectedIsValid, isValid);
        }
         
        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("user.name@domain.co.uk", true)]
        [InlineData("user_name@sub.domain.com", true)]
        [InlineData("user@domain", false)]
        [InlineData("user@.com", false)]
        [InlineData("user@domain..com", false)]
        [InlineData("user@@domain.com", false)]
        [InlineData("user domain.com", false)]
        [InlineData("user@domain.c", false)]
        [InlineData("user@c", false)]
        public void Email_Validation(string input, bool expectedIsValid)
        {
            var isValid = EmailRegex().IsMatch(input);
            Assert.Equal(expectedIsValid, isValid);
        } 
    }

}
