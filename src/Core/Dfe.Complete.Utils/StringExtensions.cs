namespace Dfe.Complete.Utils;

public static class StringExtensions
{
    public static TEnum ToEnumFromChar<TEnum>(this string value) where TEnum : struct, Enum
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Input cannot be null or empty.", nameof(value));
        
        char c = value[0];
        
        var enumCandidate = (TEnum)Enum.ToObject(typeof(TEnum), c);
        if (!Enum.IsDefined(typeof(TEnum), enumCandidate))
            throw new ArgumentException($"'{c}' (ASCII {(int)c}) is not a valid value of the enum {typeof(TEnum).Name}.", nameof(value));

        return enumCandidate;
    }
}