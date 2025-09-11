namespace Dfe.Complete.Utils
{
    public static class DateTimeExtensions
    {
        public static DateOnly? ToDateOnly(this DateTime? date)
        {
            return date.HasValue ? DateOnly.FromDateTime(date.Value) : null;
        }
    }
}
