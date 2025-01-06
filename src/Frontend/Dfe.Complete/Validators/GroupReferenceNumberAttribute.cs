using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Validators.Project;
namespace Dfe.Complete.Validators
{
    public class GroupReferenceNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var groupReferenceNumber = value as string;

            if (string.IsNullOrEmpty(groupReferenceNumber))
                return ValidationResult.Success;


            var validator = new GroupReferenceNumberValidator();
            var result = validator.Validate(groupReferenceNumber);
            if (result.IsValid)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(result.Errors[0].ErrorMessage);
            }
        }
    }
}
