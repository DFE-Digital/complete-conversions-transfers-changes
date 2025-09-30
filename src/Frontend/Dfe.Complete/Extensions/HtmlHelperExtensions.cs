using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent SimpleFormat(this IHtmlHelper html,
        string? input,
        string wrapperTag = "p",
        IDictionary<string, string>? htmlAttributes = null,
        bool htmlEncode = true)
        {
            string text = input ?? string.Empty;

            // Normalize line endings to \n
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");

            // Split into paragraphs on 2+ newlines
            string[] paragraphs = Regex.Split(text, "\n{2,}");

            var sb = new StringBuilder(text.Length + 32);
            string attributes = BuildAttributes(htmlAttributes);

            for (int i = 0; i < paragraphs.Length; i++)
            {
                string paragraph = paragraphs[i];

                // Encode for safety unless explicitly disabled
                string content = htmlEncode ? WebUtility.HtmlEncode(paragraph) : paragraph;

                // Single newlines become <br />
                content = content.Replace("\n", "<br />\n");

                sb
                    .Append('<').Append(wrapperTag)
                    .Append(attributes.Length > 0 ? " " : string.Empty)
                    .Append(attributes)
                    .Append('>')
                    .Append(content)
                    .Append("</").Append(wrapperTag).Append('>');

                if (i < paragraphs.Length - 1)
                {
                    sb.Append('\n');
                }
            }

            return new HtmlString(sb.ToString()); ;
        }

        private static string BuildAttributes(IDictionary<string, string>? attrs)
        {
            if (attrs == null || attrs.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            foreach (var kv in attrs)
            {
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;

                string name = kv.Key.Trim();
                string value = kv.Value ?? string.Empty;
                 
                value = value.Replace("\"", "&quot;");

                if (sb.Length > 0) sb.Append(' ');
                sb.Append(name).Append("=\"").Append(value).Append('"');
            }
            return sb.ToString();
        }
    }
}
