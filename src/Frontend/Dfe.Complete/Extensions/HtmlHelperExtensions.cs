using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using System.Text;

namespace Dfe.Complete.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent SimpleFormat(this IHtmlHelper html, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return HtmlString.Empty;

            var normalized = text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            var paragraphs = normalized.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();

            foreach (var para in paragraphs)
            { 
                var encoded = HtmlEncoder.Default.Encode(para);
                 
                var withBreaks = encoded.Replace("\n", "<br />");

                sb.Append("<p>").Append(withBreaks).Append("</p>");
            }

            return new HtmlString(sb.ToString());
        }
    }
}
