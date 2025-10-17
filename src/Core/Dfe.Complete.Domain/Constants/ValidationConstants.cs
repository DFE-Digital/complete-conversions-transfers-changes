namespace Dfe.Complete.Domain.Constants;

public static class ValidationConstants
{
    public const string FirstOfMonthDateValidationMessage = "{0} must be the first day of the month";
    public const string GroupReferenceNumberValidationMessage = "{0} must match format GRP_XXXXXXXX (8 digits)";
    public const string InternalEmailValidationMessage = "Email must be @education.gov.uk";
    public const string InvalidDateValidationMessage = "Email is not a valid date";
    public const string PastDateValidationMessage = "{0} must be in the past";
    public const string ProjectTypeValidationMessage = "Must be a valid project type";
    public const string UkprnValidationMessage = "UKPRN must be an 8 digit integer beginning with 1";
    public const string UrnValidationMessage = "URN must be a 6-digit integer";
}
