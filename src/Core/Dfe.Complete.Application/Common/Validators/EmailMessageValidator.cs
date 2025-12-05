using Dfe.Complete.Application.Common.Models;
using FluentValidation;

namespace Dfe.Complete.Application.Common.Validators
{
    /// <summary>
    /// Validator for EmailMessage.
    /// Ensures all required fields are present and valid before sending.
    /// </summary>
    public class EmailMessageValidator : AbstractValidator<EmailMessage>
    {
        public EmailMessageValidator()
        {
            RuleFor(x => x.To)
                .NotNull()
                .WithMessage("Email address is required");

            RuleFor(x => x.To.Value)
                .NotEmpty()
                .When(x => x.To != null)
                .WithMessage("Email address cannot be empty");

            RuleFor(x => x.TemplateKey)
                .NotEmpty()
                .WithMessage("Template key is required")
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Template key must contain only alphanumeric characters");

            RuleFor(x => x.Personalisation)
                .NotNull()
                .WithMessage("Personalisation dictionary is required");

            RuleFor(x => x.Reference)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Reference))
                .WithMessage("Reference must not exceed 100 characters");
        }
    }
}

