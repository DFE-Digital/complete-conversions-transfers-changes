using Dfe.Complete.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
       AllowMultiple = false)]
    public class ValidUkPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty((string?)value))
                return ValidationResult.Success!;
            var valueAsString = (string)value;

            var phoneRegex = new Regex(ValidationExpressions.UKPhone, RegexOptions.None, TimeSpan.FromSeconds(30));
            var match = phoneRegex.Match(valueAsString);

            return match.Success
                ? ValidationResult.Success!
                : new ValidationResult(ErrorMessage);
        }
    }
}
