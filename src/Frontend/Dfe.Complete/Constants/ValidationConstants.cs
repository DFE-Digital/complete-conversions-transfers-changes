namespace Dfe.Complete.Constants
{
    public class ValidationConstants
    {
        public const string TextValidationMessage = "The {0} must be {1} characters or less";
        public const string NumberValidationMessage = "{0} must be between {1} and {2}";
        public const string LinkValidationMessage = "The {0} must be a valid url";
        public const int LinkMaxLength = 500;
        public const string CannotBeBlank = "can't be blank";
        public const string NotRecognisedUKPostcode = "not recognised as a UK postcode";
        public const string NotRecognisedUKPhone = "not recognised as a UK phone number";
        public const string InvalidEmailFormat = "Email address must be in correct format";
    }
}
