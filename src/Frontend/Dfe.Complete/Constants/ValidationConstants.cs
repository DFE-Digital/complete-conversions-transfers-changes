namespace Dfe.Complete.Constants
{
    public static class ValidationConstants
    {
        public const string TextValidationMessage = "The {0} must be {1} characters or less";
        public const string NumberValidationMessage = "{0} must be between {1} and {2}";
        public const string LinkValidationMessage = "The {0} must be a valid url";
        public const int LinkMaxLength = 500;

        public const string NotRecognisedUKPostcode = "Not recognised as a UK postcode";
        public const string NotRecognisedUKPhone = "Not recognised as a UK phone number";
        public const string InvalidEmailFormat = "Email address must be in correct format";
        public const string AlreadyBeenTaken = "Has already been taken";
        public const string HandoverNotes = "Enter handover notes";
        public const string OutgoingSharePointLink = "Enter an outgoing trust SharePoint link";
        public const string IncomingSharePointLink = "Enter an incoming trust SharePoint link";
        public const string SchoolSharePointLink = "Enter a school SharePoint link";
        public const string TwoRequiresImprovement = "Select yes or no";
        public const string AssignedToRegionalCaseworkerTeam = "State if this project will be handed over to the Regional casework services team. Choose yes or no";
        public const string ReceptionToSixYears = "Enter the proposed capacity for pupils in reception to year 6";
        public const string SevenToElevenYears = "Enter the proposed capacity for pupils in years 7 to 11";
        public const string TwelveOrAboveYears = "Enter the proposed capacity for students in year 12 or above";
        public const string ProposedCapacityMustBeNumber = "Proposed capacity must be a number, like 345";
        public const string InvalidGuid = "{0} is not a valid Guid.";
        public const string ContactNotFound = "Contact {0} does not exist.";
        public const string ProjectNotFound = "Project {0} does not exist.";
        public const string ChooseAtLeastOneReason = "Select at least one reason";
        public const string MustProvideDetails = "You must provide details";
        public const string MinisterNameRequired = "Enter the name of the minister that approved the decision";
        public const string DecisionDateRequired = "Enter a valid date the decision was made, like 27 3 2021";
        public const string ValidDate = "Enter a valid date, like 1 1 2025";
        public const string DateInPast = "{0} date must be in the past";
        public const string MustBePastDate = "{0} must be in the past";
        // External Contact validation message
        public const string FullNameRequiredMessage = "Enter a name";
        public const string EmailRequiredMessage = "Enter an email";
        public const string InvalidEmailMessage = "Enter an email address in the correct format, like name@example.com";
        public const string RoleRequiredMessage = "Enter a role";
        public const string InvalidPrimaryContactMessage = "Only the incoming trust, outgoing trust, school or academy and local authority categories can have a primary contact.";

        // project completion validation messages
        // For transfer
        public const string IncomingTrustUkprnMissing = "The incoming trust UKPRN is entered";
        public const string TransferDateInPast = "The transfer date has been confirmed and is in the past";
        public const string AuthorityToProceedComplete = "The confirm this transfer has authority to proceed task is completed";
        public const string ExpenditureCertificateComplete = "The receive declaration of expenditure certificate task is completed";
        public const string AcademyTransferDateComplete = "The confirm the date the academy transferred task is completed";

        // For conversion
        public const string ConversionDateInPast = "The conversion date has been confirmed and is in the past";
        public const string AllConditionsMetComplete = "The confirm all conditions have been met task is completed";
        public const string AcademyOpenedDateComplete = "The confirm the date the academy opened task is completed";
        public const string RequiredSummary = "Enter the summary";
        public const string RiskProtectionArrangementOptionRequired = "Please select an option to confirm the academy's risk protection arrangements";

        // Local Authority authority validation messages       
        public const string LocalAuthorityCodeRequired = "Enter a code";
        public const string LocalAuthorityAddressLine1Required = "Enter 1st line of address";
        public const string LocalAuthorityPostcodeRequired = "Enter a postcode";

    }
}
