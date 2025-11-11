using Dfe.Complete.Constants;
using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class ValidUkPhoneNumberAttributeTests
    {
        private class PhoneValidationTestModel
        {
            [ValidUkPhoneNumber(ErrorMessage = ValidationConstants.NotRecognisedUKPhone)]
            public string? PhoneNumber { get; set; }
        }

        private static bool IsValid(PhoneValidationTestModel model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = [];
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("+44 7123 456 789")]
        [InlineData("07123 456789")]
        [InlineData("07123456789")]
        [InlineData("(07123) 456789")]
        [InlineData("+44 1234 567 890")]
        [InlineData("+441234567890")]
        [InlineData("01234 567890")]
        [InlineData("01234567890")]
        [InlineData("020 8327 3737")]
        public void ValidPhoneNumbers_ShouldPassValidation(string? phoneNumber)
        {
            var model = new PhoneValidationTestModel { PhoneNumber = phoneNumber };
            var isValid = IsValid(model, out var results);
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("123456")]
        [InlineData("INVALID")]
        [InlineData("07abc123456")]
        public void InvalidPhoneNumbers_ShouldFailValidation(string phoneNumber)
        {
            var model = new PhoneValidationTestModel { PhoneNumber = phoneNumber };
            var isValid = IsValid(model, out var results);
            Assert.False(isValid);
            Assert.Single(results);
            Assert.Equal(ValidationConstants.NotRecognisedUKPhone, results[0].ErrorMessage);
        }

        [Fact]
        public void NullPhoneNumber_ShouldPassValidation()
        {
            var model = new PhoneValidationTestModel { PhoneNumber = null };
            var isValid = IsValid(model, out var results);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }

}
