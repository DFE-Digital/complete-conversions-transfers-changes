namespace Dfe.Complete.Constants
{
    public static class ValidationExpressions
    {
        public const string UKPostCode = @"^[A-Za-z]{1,2}[0-9][0-9A-Za-z]?\s?[0-9][A-Za-z]{2}$";
        public const string UKPhone = @"^(?:(?:\+44|0044)\s?|0|\(0\d{3,5}\)\s?)(?:\d[\s-]?){6,10}(?:\s?(?:x|ext\.?|#)\d{3,4})?$";
        public const string Email = @"^[^@\s,]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$";
    }
}
