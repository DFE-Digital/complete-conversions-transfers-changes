using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public partial class AcademyUrnAttribute : ValidationAttribute
{
    [GeneratedRegex("^\\d{6}$")]
    private static partial Regex UrnRegex();
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var urn = value as string;

        if (string.IsNullOrEmpty(urn))
            return new($"Please enter an Academy URN");

        if (!UrnRegex().IsMatch(urn))
            return new ValidationResult(
                "Please enter a valid URN. The URN must be 6 digits long. For example, 123456.");

        return ValidationResult.Success;
    }
}