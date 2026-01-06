using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class UrnAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Fetch the display name if it is provided
        var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
        var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
        var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

        var urn = value as string;

        if (string.IsNullOrEmpty(urn))
            return ValidationResult.Success;

        if (urn.Length != 6)
            return new ValidationResult($"The {displayName} must be 6 digits long. For example, 123456.");

        return ValidationResult.Success;
    }
}