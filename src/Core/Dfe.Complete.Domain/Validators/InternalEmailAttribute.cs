using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class InternalEmailAttribute : ValidationAttribute
{
    public InternalEmailAttribute()
        => ErrorMessage = ValidationConstants.InternalEmailValidationMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var link = value as string;

        if (string.IsNullOrEmpty(link))
            return ValidationResult.Success;

        var emailValidator = new EmailAddressAttribute();

        if (!link.EndsWith("@education.gov.uk", StringComparison.OrdinalIgnoreCase) || !emailValidator.IsValid(value))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}