﻿using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Complete.TagHelpers
{
	public class SubMenuLinkTagHelper : AnchorTagHelper
	{
		public SubMenuLinkTagHelper(IHtmlGenerator generator) : base(generator){}

		private const string PAGE = "page";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var page = ViewContext.RouteData.Values[PAGE].ToString();
			if (page == Page)
			{
				output.Attributes.SetAttribute("aria-current", PAGE);
			}

			output.TagName = "a";

			base.Process(context, output);
		}
	}
}
