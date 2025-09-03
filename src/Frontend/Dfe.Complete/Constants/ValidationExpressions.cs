namespace Dfe.Complete.Constants
{
    public static class ValidationExpressions
    {
        public const string UKPostCode = @"^[A-Za-z]{1,2}[0-9][0-9A-Za-z]?\s?[0-9][A-Za-z]{2}$";
        public const string UKPhone = @"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$";
        public const string Email = @"^[^@\s,]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$";
    }
}
