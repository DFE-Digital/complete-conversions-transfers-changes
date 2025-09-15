namespace Dfe.Complete.Constants
{
    public static class ValidationConstants
    {
        public const string TextValidationMessage = "The {0} must be {1} characters or less";
        public const string NumberValidationMessage = "{0} must be between {1} and {2}";
        public const string LinkValidationMessage = "The {0} must be a valid url";
        public const int LinkMaxLength = 500;
        public const string CannotBeBlank = "Can't be blank";
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
        public const string ValidDate = "Enter a valid date, like 1 1 2025";
    }
}
