namespace Dfe.Complete.Constants
{
    public class ValidationConstants
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
        public const string OutgoingSharePointLink = "Enter an outgong trust SharePoint link";
        public const string IncomingSharePointLink = "Enter an incoming trust SharePoint link";
        public const string SchoolOrAcademySharePointLink = "Enter an school or academy trust SharePoint link";
        public const string TwoRequiresImprovement = "Select yes or no";
        public const string AssignedToRegionalCaseworkerTeam = "State if this project will be handed over to the Regional casework services team. Choose yes or no";
    }
}
