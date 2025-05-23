namespace Dfe.Complete.Constants
{
    public class ValidationExpressions
    {
        public const string UKPostCode = @"^[A-Z]{1,2}[0-9][0-9A-Z]?\s?[0-9][A-Z]{2}$";
        public const string UKPhone = @"^(\+44\s?7\d{3}|\(?07\d{3}\)?|\+44\s?1\d{1,4}|\(?01\d{1,4}\)?|\+44\s?2\d{1,3}|\(?02\d{1,3}\)?)\s?\d{3,4}\s?\d{3,4}$";
        public const string Email = @"^[^@\s]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$";
    }
}
