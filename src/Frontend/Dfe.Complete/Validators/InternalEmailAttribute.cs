using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class InternalEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var link = value as string;

        if (string.IsNullOrEmpty(link))
        {
            return ValidationResult.Success;
        }

        var emailValidator = new EmailAddressAttribute();
        
        if (!link.EndsWith("@education.gov.uk") || !emailValidator.IsValid(value))
        {
            var errorMessage = "Email must be @education.gov.uk";

            return new ValidationResult(errorMessage);
        }
        
        return ValidationResult.Success;
    }
}