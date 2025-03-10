using Dfe.AcademiesApi.Client.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UkprnAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public UkprnAttribute(string comparisonProperty = null)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Fetch the display name if it is provided
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

            var ukprn = value as string;

            if (string.IsNullOrEmpty(ukprn))
                return new ValidationResult("Enter a UKPRN");

            if (ukprn.Length != 8)
                return new ValidationResult(
                    $"The {displayName} must be 8 digits long and start with a 1. For example, 12345678.");
            
            if (!string.IsNullOrEmpty(_comparisonProperty))
            {
                var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (comparisonProperty == null)
                {
                    return new ValidationResult($"Property '{_comparisonProperty}' not found.");
                }

                var comparisonPropertyValue = comparisonProperty.GetValue(validationContext.ObjectInstance);

                // Compare the two values
                if (value != null && value.Equals(comparisonPropertyValue))
                {
                    return new ValidationResult($"The outgoing and incoming trust cannot be the same");
                }
            }
            
            try
            {
                var trustClient = (ITrustsV4Client)validationContext.GetService(typeof(ITrustsV4Client));
                var result = trustClient.GetTrustByUkprn2Async(ukprn).Result;
            }
            catch (AggregateException ex)
            {
                var errorMessage = $"There's no trust with that UKPRN. Check the number you entered is correct";

                return new ValidationResult(errorMessage);
            }

            // If valid, return success
            return ValidationResult.Success;
        }
    }
}