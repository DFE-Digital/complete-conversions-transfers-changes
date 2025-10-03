using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class UrnAttribute : ValidationAttribute
{
    public UrnAttribute() => ErrorMessage = ValidationConstants.UrnValidationMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is int urn && urn >= 100000 && urn <= 999999)
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }
}

