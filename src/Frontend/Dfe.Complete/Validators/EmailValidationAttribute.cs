using Dfe.Complete.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailValidationAttribute : ValidationAttribute
    {
        private static readonly Regex EmailFormatRegex = new(ValidationExpressions.Email, RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email && !EmailFormatRegex.IsMatch(email))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
