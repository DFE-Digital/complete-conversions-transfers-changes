﻿using Dfe.Complete.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Complete.TagHelpers
{
	[HtmlTargetElement("govuk-textarea-input", TagStructure = TagStructure.WithoutEndTag)]
	public class TextAreaInputTagHelper : InputTagHelperBase
	{
		[HtmlAttributeName("heading-label")]
		public bool HeadingLabel { get; set; }
		
		[HtmlAttributeName("rows")]
		public int Rows { get; set; }

		[HtmlAttributeName("rich-text")]
		public bool RichText { get; set; }

		public TextAreaInputTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper) { }

		protected override async Task<IHtmlContent> RenderContentAsync()
		{
			var model = new TextAreaInputViewModel
			{
				Id = Id,
				TestId = TestId,
				Name = Name,
				Label = Label,
				Value = For.Model?.ToString(),
				Rows = Rows,
				Hint = Hint,
				BoldLabel = BoldLabel ?? false,
				RichText = RichText,
				HeadingLabel = HeadingLabel
			};

			if (ViewContext.ModelState.TryGetValue(Name, out var entry) && entry.Errors.Count > 0)
			{
				model.ErrorMessage = entry.Errors[0].ErrorMessage;
			}

			return await _htmlHelper.PartialAsync("_TextAreaInput", model);
		}
	}
}
