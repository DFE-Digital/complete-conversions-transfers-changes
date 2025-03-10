namespace Dfe.Complete.Utils;

public static class StringExtensions
{
    public static TEnum? ToEnumFromChar<TEnum>(this string value) where TEnum : struct, Enum
    {

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Input cannot be null or empty.", nameof(value));

            char c = value[0];

            var enumCandidate = (TEnum)Enum.ToObject(typeof(TEnum), c);

            var enumName = typeof(TEnum).Name;

            if (!Enum.IsDefined(typeof(TEnum), enumCandidate))
                throw new NotFoundException($"{enumName} could not be found.", innerException: new Exception($"'{c}' (ASCII {(int)c}) is not a valid value of the enum {enumName}. {nameof(value)}"));

            return enumCandidate;
    }
}