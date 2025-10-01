using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true)]
public sealed class FirstOfMonthDateAttribute : ValidationAttribute
{
    public FirstOfMonthDateAttribute()
        => ErrorMessage = string.Format(ValidationConstants.FirstOfMonthDateValidationMessage, "Date");

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is null) return ValidationResult.Success; // compose with [Required] if needed

        int? day = value switch
        {
            DateOnly d => d.Day,
            DateTime dt => dt.Day,
            _ => null
        };

        if (day is null) return new ValidationResult("Invalid date value.");
        return day == 1 ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}
