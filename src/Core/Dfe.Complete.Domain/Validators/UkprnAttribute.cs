using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class UkprnAttribute : ValidationAttribute
{
    public bool ValueIsInteger { get; init; } = false;

    public UkprnAttribute() => ErrorMessage = ValidationConstants.UkprnValidationMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (ValueIsInteger)
        {
            if (value is int urn && urn >= 10000000 && urn <= 19999999)
                return ValidationResult.Success;
        }
        else
        {
            if (value is Ukprn ukprn && ukprn.Value >= 10000000 && ukprn.Value <= 19999999)
                return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }
}
