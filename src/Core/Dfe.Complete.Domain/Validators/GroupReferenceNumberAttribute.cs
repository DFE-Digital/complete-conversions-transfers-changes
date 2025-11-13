using Dfe.Complete.Domain.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class GroupReferenceNumberAttribute : ValidationAttribute
{
    private const string Pattern = @"^GRP_\d{8}$";
    private static readonly Regex RegexCompiled = new(
        Pattern,
        RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromMilliseconds(100));

    public GroupReferenceNumberAttribute()
        => ErrorMessage = string.Format(ValidationConstants.GroupReferenceNumberValidationMessage, "Group ID");

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        if (value is string s)
        {
            if (s.Length == 0) return new ValidationResult(ErrorMessage);
            if (RegexCompiled.IsMatch(s)) return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }
}
