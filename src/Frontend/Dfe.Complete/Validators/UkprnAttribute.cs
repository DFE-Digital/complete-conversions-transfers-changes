using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Services;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Validators
{
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
            {
                var errorMessage = $"Enter a UKPRN";

                return new ValidationResult(errorMessage);
            }

            if (ukprn.Length != 8)
            {
                var errorMessage = $"The {displayName} must be 8 digits long and start with a 1. For example, 12345678.";

                return new ValidationResult(errorMessage);
            }

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
                var trustClient = (ITrustsClient)validationContext.GetService(typeof(ITrustsClient));
                var result = trustClient.GetTrustByUkprnAsync(ukprn, "v4").Result;
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
