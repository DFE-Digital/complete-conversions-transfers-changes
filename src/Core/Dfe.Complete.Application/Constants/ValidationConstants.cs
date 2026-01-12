namespace Dfe.Complete.Application.Constants;

public static class ValidationConstants
{
    public const string MismatchedTrustInGroupValidationMessage = "Trust UKPRN {0} is not the same as the group UKPRN for group {1}";
    public const string NoTrustFoundValidationMessage = "There's no trust with UKPRN {0}. Check the number you entered is correct";
    public const string SameTrustValidationMessage = "Incoming trust UKPRN cannot be the same as the outgoing trust UKPRN";
    public const string UrnExistsValidationMessage = "URN {0} already exists in active/inactive projects";
}
