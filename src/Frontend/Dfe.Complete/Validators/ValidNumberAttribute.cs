using Dfe.Complete.Constants;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class ValidNumberAttribute(int minValue, int maxValue, string dependentProperty = "") : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        bool? dependentValue = false;
        if (!string.IsNullOrWhiteSpace(dependentProperty))
        {
            var property = validationContext.ObjectType.GetProperty(dependentProperty);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {dependentProperty}");
            }
            dependentValue = property.GetValue(validationContext.ObjectInstance, null) as bool?;
        }
        if (dependentValue != true)
        {
            if (value is null)
                return ValidationResult.Success!;

            var valueAsString = (string)value;

            if (string.IsNullOrWhiteSpace(valueAsString))
                return ValidationResult.Success!;

            bool success = int.TryParse(valueAsString, out int valueAsInt);

            if (!success)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be a number, like 30");

            if (valueAsInt < minValue || valueAsInt > maxValue)
                return new ValidationResult(ErrorMessage ?? string.Format(ValidationConstants.NumberValidationMessage, validationContext.DisplayName, minValue, maxValue));
        }
        return ValidationResult.Success!;
    }
}