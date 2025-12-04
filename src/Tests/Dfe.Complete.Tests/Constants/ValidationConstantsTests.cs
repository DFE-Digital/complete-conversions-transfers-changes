using Dfe.Complete.Constants;

namespace Dfe.Complete.Tests.Constants
{
    public class ValidationConstantsTests
    {
        [Fact]
        public void TextValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Name";
            var maxLength = "50";
            var expected = $"The {fieldName} must be {maxLength} characters or less";
            var actual = string.Format(ValidationConstants.TextValidationMessage, fieldName, maxLength);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NumberValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Age";
            var min = "18";
            var max = "65";
            var expected = $"{fieldName} must be between {min} and {max}";
            var actual = string.Format(ValidationConstants.NumberValidationMessage, fieldName, min, max);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LinkValidationMessage_ShouldFormatCorrectly()
        {
            var fieldName = "Website";
            var expected = $"The {fieldName} must be a valid url";
            var actual = string.Format(ValidationConstants.LinkValidationMessage, fieldName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LinkMaxLength_ShouldReturnExpectedValue()
        {
            Assert.Equal(500, ValidationConstants.LinkMaxLength);
        }

        [Fact]
        public void NotRecognisedUKPostcode_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Not recognised as a UK postcode", ValidationConstants.NotRecognisedUKPostcode);
        }

        [Fact]
        public void NotRecognisedUKPhone_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Not recognised as a UK phone number", ValidationConstants.NotRecognisedUKPhone);
        }

        [Fact]
        public void InvalidEmailFormat_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Email address must be in correct format", ValidationConstants.InvalidEmailFormat);
        }
        [Fact]
        public void AlreadyBeenTaken_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Has already been taken", ValidationConstants.AlreadyBeenTaken);
        }
        [Fact]
        public void HandoverNotes_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter handover notes", ValidationConstants.HandoverNotes);
        }
        [Fact]
        public void OutgoingSharePointLink_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter an outgoing trust SharePoint link", ValidationConstants.OutgoingSharePointLink);
        }
        [Fact]
        public void IncomingSharePointLink_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter an incoming trust SharePoint link", ValidationConstants.IncomingSharePointLink);
        }
        [Fact]
        public void SchoolSharePointLink_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter a school SharePoint link", ValidationConstants.SchoolSharePointLink);
        }
        [Fact]
        public void TwoRequiresImprovement_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Select yes or no", ValidationConstants.TwoRequiresImprovement);
        }
        [Fact]
        public void AssignedToRegionalCaseworkerTeam_ShouldReturnExpectedMessage()
        {
            Assert.Equal("State if this project will be handed over to the Regional casework services team. Choose yes or no", ValidationConstants.AssignedToRegionalCaseworkerTeam);
        }
        [Fact]
        public void ReceptionToSixYears_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter the proposed capacity for pupils in reception to year 6", ValidationConstants.ReceptionToSixYears);
        }
        [Fact]
        public void SevenToElevenYears_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter the proposed capacity for pupils in years 7 to 11", ValidationConstants.SevenToElevenYears);
        }
        [Fact]
        public void TwelveOrAboveYears_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter the proposed capacity for students in year 12 or above", ValidationConstants.TwelveOrAboveYears);
        }
        [Fact]
        public void ProposedCapacityMustBeNumber_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Proposed capacity must be a number, like 345", ValidationConstants.ProposedCapacityMustBeNumber);
        }
        [Fact]
        public void ValidDate_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter a valid date, like 1 1 2025", ValidationConstants.ValidDate);
        }
        [Fact]
        public void ChooseAtLeastOneReason_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Select at least one reason", ValidationConstants.ChooseAtLeastOneReason);
        }
        [Fact]
        public void MustProvideDetails_ShouldReturnExpectedMessage()
        {
            Assert.Equal(
                "You must provide details", ValidationConstants.MustProvideDetails);
        }
        [Fact]
        public void MinisterNameRequired_ShouldReturnExpectedMessage()
        {
            Assert.Equal(
                "Enter the name of the minister that approved the decision", ValidationConstants.MinisterNameRequired);
        }
        [Fact]
        public void DecisionDateRequired_ShouldReturnExpectedMessage()
        {
            Assert.Equal(
                "Enter a valid date the decision was made, like 27 3 2021", ValidationConstants.DecisionDateRequired);
        }

        [Fact]
        public void TransferDateInPast_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The transfer date has been confirmed and is in the past", ValidationConstants.TransferDateInPast);
        }

        [Fact]
        public void AuthorityToProceedComplete_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The confirm this transfer has authority to proceed task is completed", ValidationConstants.AuthorityToProceedComplete);
        }

        [Fact]
        public void IncomingTrustUkprnMissing_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The incoming trust UKPRN is entered", ValidationConstants.IncomingTrustUkprnMissing);
        }

        [Fact]
        public void ExpenditureCertificateComplete_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The receive declaration of expenditure certificate task is completed", ValidationConstants.ExpenditureCertificateComplete);
        }

        [Fact]
        public void AcademyTransferDateComplete_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The confirm the date the academy transferred task is completed", ValidationConstants.AcademyTransferDateComplete);
        }

        [Fact]
        public void ConversionDateInPast_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The conversion date has been confirmed and is in the past", ValidationConstants.ConversionDateInPast);
        }

        [Fact]
        public void AllConditionsMetComplete_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The confirm all conditions have been met task is completed", ValidationConstants.AllConditionsMetComplete);
        }

        [Fact]
        public void AcademyOpenedDateComplete_ShouldReturnExpectedMessage()
        {
            Assert.Equal("The confirm the date the academy opened task is completed", ValidationConstants.AcademyOpenedDateComplete);
        }
        [Fact]
        public void DateInPast_ShouldReturnExpectedMessage()
        {
            var dateField = "Opened academy";
            Assert.Equal($"{dateField} date must be in the past", string.Format(ValidationConstants.DateInPast, dateField));
        }
        [Fact]
        public void MustBePastDate_ShouldReturnExpectedMessage()
        {
            var dateField = "Opened academy date";
            Assert.Equal($"{dateField} must be in the past", string.Format(ValidationConstants.MustBePastDate, dateField));
        }

        [Fact]
        public void LocalAuthorityCodeRequired_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter a code", ValidationConstants.LocalAuthorityCodeRequired);
        }

        [Fact]
        public void LocalAuthorityAddressLine1Required_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter 1st line of address", ValidationConstants.LocalAuthorityAddressLine1Required);
        }

        [Fact]
        public void LocalAuthorityPostcodeRequired_ShouldReturnExpectedMessage()
        {
            Assert.Equal("Enter a postcode", ValidationConstants.LocalAuthorityPostcodeRequired);
        }

    }
}
