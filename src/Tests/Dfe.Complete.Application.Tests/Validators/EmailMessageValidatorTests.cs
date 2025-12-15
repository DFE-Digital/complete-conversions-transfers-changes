using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Common.Validators;
using Dfe.Complete.Domain.ValueObjects;
using FluentValidation.TestHelper;
using Xunit;

namespace Dfe.Complete.Application.Tests.Validators
{
    /// <summary>
    /// Tests for EmailMessageValidator
    /// Validates FluentValidation rules for EmailMessage
    /// </summary>
    public class EmailMessageValidatorTests
    {
        private readonly EmailMessageValidator _validator;

        public EmailMessageValidatorTests()
        {
            _validator = new EmailMessageValidator();
        }

        [Fact]
        public void Validate_WithValidMessage_PassesValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate123",
                Personalisation: new Dictionary<string, string> { { "key", "value" } },
                Reference: "test-ref-123"
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithNullEmailAddress_FailsValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: null!,
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.To)
                .WithErrorMessage("Email address is required");
        }

        [Fact]
        public void Validate_WithEmptyTemplateKey_FailsValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TemplateKey)
                .WithErrorMessage("Template key is required");
        }

        [Fact]
        public void Validate_WithInvalidTemplateKeyCharacters_FailsValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "Invalid-Template-Key!", // Contains hyphens and exclamation
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TemplateKey)
                .WithErrorMessage("Template key must contain only alphanumeric characters");
        }

        [Fact]
        public void Validate_WithValidAlphanumericTemplateKey_PassesValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate123",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TemplateKey);
        }

        [Fact]
        public void Validate_WithNullPersonalisation_FailsValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: null!,
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Personalisation)
                .WithErrorMessage("Personalisation dictionary is required");
        }

        [Fact]
        public void Validate_WithEmptyPersonalisation_PassesValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(), // Empty but not null
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Personalisation);
        }

        [Fact]
        public void Validate_WithReferenceTooLong_FailsValidation()
        {
            // Arrange
            var longReference = new string('x', 101); // 101 characters
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: longReference
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Reference)
                .WithErrorMessage("Reference must not exceed 100 characters");
        }

        [Fact]
        public void Validate_WithReferenceExactly100Characters_PassesValidation()
        {
            // Arrange
            var exactReference = new string('x', 100); // Exactly 100 characters
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: exactReference
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Reference);
        }

        [Fact]
        public void Validate_WithNullReference_PassesValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Reference);
        }

        [Fact]
        public void Validate_WithWhitespaceReference_PassesValidation()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "ValidTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "   " // Whitespace only
            );

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Reference);
        }
    }
}

