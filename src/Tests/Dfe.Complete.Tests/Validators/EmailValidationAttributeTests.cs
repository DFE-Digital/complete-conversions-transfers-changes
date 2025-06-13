using Dfe.Complete.Constants;
using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class EmailValidationAttributeTests
    {
        private class ValidateEmailTestModel
        {
            [EmailValidation(ErrorMessage = ValidationConstants.InvalidEmailFormat)]
            public string? Email { get; set; }
        }

        private static bool IsValid(ValidateEmailTestModel model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = [];
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("user_name@sub.domain.com")]
        public void ValidEmails_ShouldPassValidation(string email)
        {
            var model = new ValidateEmailTestModel { Email = email };
            var isValid = IsValid(model, out var results);
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("user@")]
        [InlineData("user@.com")]
        [InlineData("user@domain")]
        [InlineData("user@domain..com")]
        [InlineData("user@@domain.com")]
        [InlineData("user domain.com")]
        public void InvalidEmails_ShouldFailValidation(string email)
        {
            var model = new ValidateEmailTestModel { Email = email };
            var isValid = IsValid(model, out var results);
            Assert.False(isValid);
            Assert.Single(results);
            Assert.Equal(ValidationConstants.InvalidEmailFormat, results[0].ErrorMessage);
        }

        [Fact]
        public void NullEmail_ShouldPassValidation()
        {
            var model = new ValidateEmailTestModel { Email = null };
            var isValid = IsValid(model, out var results);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }

}
