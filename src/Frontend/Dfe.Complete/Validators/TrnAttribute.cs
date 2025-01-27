using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators;

public partial class TrnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Fetch the display name if it is provided
        var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
        var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
        var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

        var trn = value as string;

        if (string.IsNullOrEmpty(trn))
            return new ValidationResult("Enter a Trust reference number (TRN)");

        if (TrnRegex().IsMatch(trn) == false)
            return new ValidationResult("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234");

        
        
        
        
        return base.IsValid(value, validationContext);
    }

    [GeneratedRegex("^TR\\d{5}$")]
    private static partial Regex TrnRegex();
}