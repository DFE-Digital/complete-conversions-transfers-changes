using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Dfe.Complete.Validators
{
    public class GroupReferenceNumberAttribute : ValidationAttribute
    {
        private readonly bool _shouldMatchWithTrustUkprn;
        private readonly string _UkprnField;

        public GroupReferenceNumberAttribute(bool ShouldMatchWithTrustUkprn = false, string ukprnField = null)
        {
            _shouldMatchWithTrustUkprn = ShouldMatchWithTrustUkprn;
            _UkprnField = ukprnField;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var groupReferenceNumber = value as string;

            if (string.IsNullOrEmpty(groupReferenceNumber))
                return ValidationResult.Success;

            string errorMessage = "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001";

            if (!groupReferenceNumber.StartsWith("GRP_"))
            {
                return new ValidationResult(errorMessage);
            }

            var numberPortionOfRefNumber = groupReferenceNumber.Split("GRP_")[1];

            if (!int.TryParse(numberPortionOfRefNumber, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                return new ValidationResult(errorMessage);

            if (numberPortionOfRefNumber.Length != 8)
                return new ValidationResult(errorMessage);

            if (_shouldMatchWithTrustUkprn == true)
            {
                //Get the value of the  ukprn
                var ukprnProperty = validationContext.ObjectType.GetProperty(_UkprnField);

                if (ukprnProperty == null)
                {
                    return new ValidationResult($"Property '{_UkprnField}' not found.");
                }

                var ukprnValue = ukprnProperty.GetValue(validationContext.ObjectInstance)?.ToString();

                if (string.IsNullOrEmpty(ukprnValue))
                {
                    return new ValidationResult($"Incoming trust ukprn cannot be empty");
                }

                if (!IsGroupReferenceForSameTrust(groupReferenceNumber, ukprnValue, validationContext))
                {
                    errorMessage = $"The group reference number must be for the same trust as all other group members, check the group reference number and incoming trust UKPRN";

                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private bool IsGroupReferenceForSameTrust(string groupReferenceNumber, string? incomingUkprn, ValidationContext validationContext)
        {
            var sender = (ISender)validationContext.GetService(typeof(ISender));

            var result = sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(groupReferenceNumber)).Result;
            var ukprn = result.Value?.TrustUkprn.Value.ToString();

            if (result != null && incomingUkprn.Equals(ukprn))
            {
                return true;
            }

            return false;
        }

    }
}
