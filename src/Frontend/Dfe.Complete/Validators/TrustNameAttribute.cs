using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TrustNameAttribute(string trustReference) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var trustName = value as string ?? string.Empty;
        var trnProperty = validationContext.ObjectType.GetProperty(trustReference);

        if (trnProperty == null)
        {
            return new ValidationResult($"Property '{trustReference}' not found.");
        }

        var trn = trnProperty.GetValue(validationContext.ObjectInstance)?.ToString();

        if (string.IsNullOrEmpty(trustName) && string.IsNullOrEmpty(trn))
        {
            return new ValidationResult("Enter a trust name.");
        }

        if (string.IsNullOrEmpty(trustName))
        {
            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}