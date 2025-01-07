using System.ComponentModel;
using System.Reflection;

namespace Dfe.Complete.Utils
{
	public static class EnumExtensions
	{
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
		
		public static string ToIntString(this Enum value)
		{
			if (value == null) return string.Empty;

			return value.ToString("D");
		}

		public static T? ToEnum<T>(this string value) where T : struct
        {
			if (value == null) return null;

            return (T)Enum.Parse(typeof(T), value);
		}

	}
}
