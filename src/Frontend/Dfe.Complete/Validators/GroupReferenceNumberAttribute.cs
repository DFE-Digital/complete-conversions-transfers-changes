using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Dfe.Complete.Validators
{
    public class GroupReferenceNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var groupReferenceNumber = value as string;

            if (string.IsNullOrEmpty(groupReferenceNumber))
                return ValidationResult.Success;

            const string errorMessage = "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001";

            if (!groupReferenceNumber.StartsWith("GRP_"))
            {
                return new ValidationResult(errorMessage);
            }

            var numberPortionOfRefNumber = groupReferenceNumber.Split("GRP_")[1];

            if (!int.TryParse(numberPortionOfRefNumber, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                return new ValidationResult(errorMessage);

            if(numberPortionOfRefNumber.Length != 8)
                return new ValidationResult(errorMessage);

            return ValidationResult.Success;
        }


    }
}
