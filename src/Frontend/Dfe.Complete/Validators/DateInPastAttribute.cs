using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators
{
    public class DateInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Fetch the display name if it is provided
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;
            
            var date = value as DateTime?;

            if (date == null)
            {
                return ValidationResult.Success;
            }

            if (date >= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? $"The {displayName} date must be in the past.");
            }

            // If valid, return success
            return ValidationResult.Success;
        }
    }
}
