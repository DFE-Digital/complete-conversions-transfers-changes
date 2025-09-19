using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Dfe.Complete.Validators
{
    public class GroupReferenceNumberAttribute(bool ShouldMatchWithTrustUkprn = false, string? ukprnField = null) : ValidationAttribute
    {
        private readonly bool _shouldMatchWithTrustUkprn = ShouldMatchWithTrustUkprn;
        private readonly string? _UkprnField = ukprnField;

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var groupReferenceNumber = value as string;
            if (string.IsNullOrEmpty(groupReferenceNumber))
                return ValidationResult.Success!;

            string errorMessage = "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001";
            if (!groupReferenceNumber.StartsWith("GRP_"))
                return new ValidationResult(errorMessage);

            var splitParts = groupReferenceNumber.Split("GRP_");
            if (splitParts.Length != 2)
                return new ValidationResult(errorMessage);

            var numberPortionOfRefNumber = splitParts[1];
            if (!int.TryParse(numberPortionOfRefNumber, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                return new ValidationResult(errorMessage);
            if (numberPortionOfRefNumber.Length != 8)
                return new ValidationResult(errorMessage);

            if (_shouldMatchWithTrustUkprn)
                return ValidateTrustUkprn(validationContext, groupReferenceNumber);

            return ValidationResult.Success!;
        }

        private ValidationResult ValidateTrustUkprn(ValidationContext validationContext, string groupReferenceNumber)
        {
            if (string.IsNullOrEmpty(_UkprnField))
                return new ValidationResult("UKPRN field name is not specified.");

            var ukprnProperty = validationContext.ObjectType.GetProperty(_UkprnField);
            if (ukprnProperty == null)
                return new ValidationResult($"Property '{_UkprnField}' not found.");

            var incomingUkprnValue = ukprnProperty.GetValue(validationContext.ObjectInstance)?.ToString();
            if (string.IsNullOrEmpty(incomingUkprnValue))
                return new ValidationResult("Incoming trust ukprn cannot be empty");

            var senderObj = validationContext.GetService(typeof(ISender));
            if (senderObj is not ISender sender)
                return new ValidationResult("ISender service is not available in validation context.");

            var result = sender.Send(new GetProjectGroupByGroupReferenceNumberQuery(groupReferenceNumber)).Result;
            if (result == null || result.Value == null)
                return ValidationResult.Success!;

            var trustUkprnObj = result.Value?.TrustUkprn;
            var ukprn = trustUkprnObj?.Value.ToString();
            if (ukprn != null && incomingUkprnValue.Equals(ukprn))
                return ValidationResult.Success!;

            return new ValidationResult("The group reference number must be for the same trust as all other group members, check the group reference number and incoming trust UKPRN");
        }
    }
}