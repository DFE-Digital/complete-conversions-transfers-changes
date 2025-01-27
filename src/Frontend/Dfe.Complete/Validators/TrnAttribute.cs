using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators;

public partial class TrnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var trn = value as string;

        if (string.IsNullOrEmpty(trn))
            return new ValidationResult("Enter a Trust reference number (TRN)");

        if (TrnRegex().IsMatch(trn) == false)
            return new ValidationResult("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234");
        
        return ValidationResult.Success;    
    }

    [GeneratedRegex("^TR\\d{5}$")]
    private static partial Regex TrnRegex();
}