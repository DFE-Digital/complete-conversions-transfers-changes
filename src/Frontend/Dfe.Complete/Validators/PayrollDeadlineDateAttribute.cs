using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators
{
    public class PayrollDeadlineDateAttribute : ValidationAttribute
    {
        public string SignificantDatePropertyName { get; set; }

        public PayrollDeadlineDateAttribute(string significantDatePropertyName)
        {
            SignificantDatePropertyName = significantDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Fetch the display name if it is provided
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

            var date = value as DateOnly?;

            if (date == null)
            {
                return ValidationResult.Success;
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
                if (significantDateValue is DateOnly significantDate)
                {
                    if (date >= significantDate)
                    {
                        return new ValidationResult($"The {displayName} must be before the significant date.");
                    }
                }
            }

            // If valid, return success
            return ValidationResult.Success;
        }
    }
}