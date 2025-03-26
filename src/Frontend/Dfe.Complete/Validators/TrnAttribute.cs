using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public partial class TrnAttribute : ValidationAttribute
{
    [GeneratedRegex("^TR\\d{5}$")]
    private static partial Regex TrnRegex();
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var trn = value as string;

        if (!string.IsNullOrEmpty(trn) && !TrnRegex().IsMatch(trn))
            return new ValidationResult(
                "The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234");

        return ValidationResult.Success;
    }
}