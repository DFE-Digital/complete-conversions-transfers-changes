using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true)]
public sealed class PastDateAttribute : ValidationAttribute
{
    public bool AllowToday { get; init; } = false;
    public PastDateAttribute()
        => ErrorMessage = string.Format(ValidationConstants.PastDateValidationMessage, "Date");

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is null) return ValidationResult.Success;

        var today = DateOnly.FromDateTime(DateTime.Today);

        DateOnly candidate = value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => default
        };

        if (candidate == default && value is not DateOnly && value is not DateTime)
            return new ValidationResult(ValidationConstants.InvalidDateValidationMessage);

        bool ok = AllowToday ? candidate <= today : candidate < today;
        return ok ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}
