using System.ComponentModel;
using System.Reflection;

namespace Dfe.Complete.Utils;

public static class EnumExtensions
{
	public static string GetCharValue<TEnum>(this TEnum? enumValue) where TEnum : struct, Enum
	{
		return enumValue.HasValue ? ((char)Convert.ToUInt16(enumValue.Value)).ToString() : string.Empty;
	}
		
	public static string ToDescription<T>(this T source)
	{
		if (source == null) 
			return string.Empty;

		var fi = source.GetType().GetField(source.ToString() ?? string.Empty);

		if (fi == null) 
			return string.Empty;
        
		var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

		return (attributes.Length > 0 ? attributes[0].Description : source.ToString()) ?? string.Empty;
	}
		
	public static T? FromDescription<T>(string? description) where T : Enum
	{
		if (string.IsNullOrEmpty(description))
			throw new ArgumentException("Description cannot be null or empty.", nameof(description));

		foreach (var field in typeof(T).GetFields())
		{
			if (field.IsLiteral) 
			{
				var attribute = field.GetCustomAttribute<DescriptionAttribute>();
				if (attribute != null && attribute.Description == description)
					return (T)field.GetValue(null);

				var fieldValue = field.GetValue(null)?.ToString();
				if (fieldValue == description)
					return (T)field.GetValue(null);
			}
		}

		return default;
	}
		
	public static TEnum FromDescriptionValue<TEnum>(this string? description)
		where TEnum : struct, Enum
	{
		if (string.IsNullOrEmpty(description))
			throw new ArgumentNullException(nameof(description));

		// Look for an enum value whose [Description] matches the input string
		foreach (var value in Enum.GetValues(typeof(TEnum)))
		{
			var enumValue = (TEnum)value;
			var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());
			var descriptionAttribute = fieldInfo
				?.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.FirstOrDefault() as DescriptionAttribute;

			if (descriptionAttribute?.Description == description)
				return enumValue;
		}

		// Fallback: maybe it matches the enum name?
		if (Enum.TryParse(description, out TEnum parsed))
			return parsed;

		throw new ArgumentException($"No matching enum value found for '{description}'.");
	}
}