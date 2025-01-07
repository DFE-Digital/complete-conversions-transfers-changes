using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.Extensions;

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
}