using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PayrollDeadlineDateAttribute(string significantDatePropertyName) : ValidationAttribute
    {
        public string SignificantDatePropertyName { get; set; } = significantDatePropertyName;

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // Fetch the display name if it is provided
            PropertyInfo? property = null;
            if (!string.IsNullOrEmpty(validationContext.MemberName))
            {
                property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            }
            var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

            var date = value as DateOnly?;

            if (date == null)
            {
                return ValidationResult.Success!;
            }

            // Check if date is in the future
            if (date <= DateOnly.FromDateTime(DateTime.Now))
            {
                return new ValidationResult($"The {displayName} must be in the future.");
            }

            // Get the significant date property value
            var significantDateProperty = validationContext.ObjectType.GetProperty(SignificantDatePropertyName);
            if (significantDateProperty != null)
            {
                var significantDateValue = significantDateProperty.GetValue(validationContext.ObjectInstance);
                if (significantDateValue is DateOnly significantDate && date >= significantDate)
                {
                    return new ValidationResult($"The {displayName} must be before the significant date.");
                }
            }

            // If valid, return success
            return ValidationResult.Success!;
        }
    }
}