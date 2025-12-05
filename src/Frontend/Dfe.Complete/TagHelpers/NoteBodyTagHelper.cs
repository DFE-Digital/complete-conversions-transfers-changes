
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.TagHelpers;

[ExcludeFromCodeCoverage]
[HtmlTargetElement("note-body")]
public class NoteBodyTagHelper : TagHelper
{
    /// <summary>
    /// Additional CSS classes to apply to the note body element
    /// </summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "govuk-body-m";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "pre";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", CssClass);
        output.Attributes.SetAttribute("style", "white-space: pre-line;");
    }
}