namespace Dfe.Complete.Application.Constants;

public static class ValidationConstants
{
    public const string MismatchedTrustInGroupValidationMessage = "Trust UKPRN {0} is not the same as the group UKPRN for group {1}";
    public const string NoTrustFoundValidationMessage = "There's no trust with that UKPRN. Check the number you entered is correct";
    public const string UrnExistsValidationMessage = "URN {0} already exists in active/inactive conversion projects";
}
