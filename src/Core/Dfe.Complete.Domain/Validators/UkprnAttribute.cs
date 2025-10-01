using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class UkprnAttribute : ValidationAttribute
{
    public UkprnAttribute() => ErrorMessage = ValidationConstants.UkprnValidationMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is int urn)
            if (urn >= 10000000 && urn <= 19999999)
                return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }
}

