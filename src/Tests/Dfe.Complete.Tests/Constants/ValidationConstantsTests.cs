using Dfe.Complete.Constants;

namespace Dfe.Complete.Tests.Constants
{
    public class ValidationConstantsTests
    {
        [Fact]
        public void TextValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Name";
            var maxLength = "50";
            var expected = $"The {fieldName} must be {maxLength} characters or less";
            var actual = string.Format(ValidationConstants.TextValidationMessage, fieldName, maxLength);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NumberValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Age";
            var min = "18";
            var max = "65";
            var expected = $"{fieldName} must be between {min} and {max}";
            var actual = string.Format(ValidationConstants.NumberValidationMessage, fieldName, min, max);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LinkValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Website";
            var expected = $"The {fieldName} must be a valid url";
            var actual = string.Format(ValidationConstants.LinkValidationMessage, fieldName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LinkMaxLength_ShouldReturnExpectedValue()
        {
            Assert.Equal(500, ValidationConstants.LinkMaxLength);
        }

        [Fact]
        public void CannotBeBlank_ShouldReturnExpectedMessage()
        {
            Assert.Equal("can't be blank", ValidationConstants.CannotBeBlank);
        }

        [Fact]
        public void NotRecognisedUKPostcode_ShouldReturnExpectedMessage()
        {
            Assert.Equal("not recognised as a UK postcode", ValidationConstants.NotRecognisedUKPostcode);
        }

        [Fact]
        public void NotRecognisedUKPhone_ShouldReturnExpectedMessage()
        {
            Assert.Equal("not recognised as a UK phone number", ValidationConstants.NotRecognisedUKPhone);
        }

        [Fact]
        public void InvalidEmailFormat_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Email address must be in correct format", ValidationConstants.InvalidEmailFormat);
        }
    }
}
