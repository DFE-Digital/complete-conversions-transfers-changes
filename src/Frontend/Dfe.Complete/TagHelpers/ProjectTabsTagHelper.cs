﻿using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.TagHelpers
{
    [HtmlTargetElement("govuk-project-tabs", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ProjectTabsTagHelper : TagHelper
    {
        [HtmlAttributeName("project-id")]
        public string ProjectId { get; set; }

        [HtmlAttributeName("project-type")]
        public ProjectType ProjectType { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-grid-row");
            
            string notesLink = string.Format(Constants.RouteConstants.ProjectViewNotes, ProjectId);

            var aboutProjectLink = string.Format(Constants.RouteConstants.ProjectAbout, ProjectId);
            var taskListLink = string.Format(Constants.RouteConstants.ProjectTaskList, ProjectId);
            var internalContactsLink = string.Format(Constants.RouteConstants.ProjectInternalContacts, ProjectId);

            var currentPageAttribute = @"aria-current=""page""";
            var currentUrl = ViewContext.HttpContext.Request.GetDisplayUrl();

            var aboutPageCurrentStyle = currentUrl.Contains(aboutProjectLink) ? currentPageAttribute : "";
            var taskListCurrentPageStyle = currentUrl.Contains(taskListLink) ? currentPageAttribute : "";
            var notesCurrentPageStyle = currentUrl.Contains(notesLink) ? currentPageAttribute : "";
            var internalContactsCurrentPageStyle = currentUrl.Contains(internalContactsLink) ? currentPageAttribute : "";

            output.Content.SetHtmlContent(
                $@"<div class=""govuk-grid-column-full"">
                    <nav class=""moj-sub-navigation"" aria-label=""Project sub-navigation"">
                        <ul class=""moj-sub-navigation__list"">

                            <li class=""moj-sub-navigation__item"">
                                <a class=""moj-sub-navigation__link"" {taskListCurrentPageStyle} href=""{taskListLink}"">Task list</a>
                            </li>


                            <li class=""moj-sub-navigation__item"">
                                <a class=""moj-sub-navigation__link"" {aboutPageCurrentStyle} href=""{aboutProjectLink}"">About the project</a>
                            </li>


                            <li class=""moj-sub-navigation__item"">
                                <a class=""moj-sub-navigation__link"" {notesCurrentPageStyle} href=""{notesLink}"">Notes</a>
                            </li>


                            <li class=""moj-sub-navigation__item"">
                                <a class=""moj-sub-navigation__link"" href=""/projects/51FA403B-B3F3-4580-919B-207B842B9BE3/external-contacts"">External contacts</a>
                            </li>


                            <li class=""moj-sub-navigation__item"">
                                <a class=""moj-sub-navigation__link"" {internalContactsCurrentPageStyle} href=""{internalContactsLink}"">Internal contacts</a>
                            </li>


                        </ul>
                    </nav>
                </div>
            ");

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
