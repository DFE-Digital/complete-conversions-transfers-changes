using Microsoft.AspNetCore.Razor.TagHelpers;
using Dfe.Complete.TagHelpers;

namespace Dfe.Complete.TagHelpers.Tests
{
    public class ExternalLinkTagHelperTest
    {
        private static TagHelperContext CreateTagHelperContext() =>
            new(
                tagName: "external-link",
                allAttributes: [],
                items: new Dictionary<object, object>(),
                uniqueId: "test");

        private static TagHelperOutput CreateTagHelperOutput() =>
            new(
                "external-link",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent().SetContent("Test Link")));

        [Fact]
        public void Process_WithNullHref_SuppressesOutput()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = null
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.True(output.IsContentModified);
            Assert.True(output.TagName == null);
        }

        [Fact]
        public void Process_WithEmptyHref_SuppressesOutput()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = ""
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.True(output.IsContentModified);
            Assert.True(output.TagName == null);
        }

        [Fact]
        public void Process_WithValidHref_CreatesAnchorTag()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com"
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("a", output.TagName);
        }

        [Fact]
        public void Process_WithValidHref_SetsHrefAttribute()
        {
            // Arrange
            var testUrl = "https://example.com";
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = testUrl
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal(testUrl, output.Attributes["href"].Value);
        }

        [Fact]
        public void Process_WithValidHref_SetsTargetBlank()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com"
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("_blank", output.Attributes["target"].Value);
        }

        [Fact]
        public void Process_WithValidHref_SetsRelAttribute()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com"
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("noopener noreferrer", output.Attributes["rel"].Value);
        }

        [Fact]
        public void Process_WithNoCssClass_SetsDefaultGovUkLinkClass()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com"
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("govuk-link", output.Attributes["class"].Value);
        }

        [Fact]
        public void Process_WithCustomCssClass_SetsCustomClass()
        {
            // Arrange
            var customClass = "custom-link-class";
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                CssClass = customClass
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal(customClass, output.Attributes["class"].Value);
        }

        [Fact]
        public void Process_WithShowNewTabTextTrue_AddsOpensInNewTabText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Contains("(opens in new tab)", output.Content.GetContent());
        }

        [Fact]
        public void Process_WithShowNewTabTextFalse_DoesNotAddOpensInNewTabText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = false
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.DoesNotContain("(opens in new tab)", output.Content.GetContent());
            Assert.Equal("Test Link", output.Content.GetContent());
        }

        [Fact]
        public void Process_WithCustomScreenReaderDescription_UsesCustomDescription()
        {
            // Arrange
            var customDescription = "(opens in new window)";
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true,
                ScreenReaderDescription = customDescription
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Contains(customDescription, output.Content.GetContent());
            Assert.DoesNotContain("(opens in new tab)", output.Content.GetContent());
        }

        [Fact]
        public void Process_WithContentAlreadyContainingOpensIn_DoesNotAddDuplicateText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true
            };
            var context = CreateTagHelperContext();
            var output = new TagHelperOutput(
                "external-link",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent().SetContent("Link opens in new tab")));

            // Act
            tagHelper.Process(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.Equal("Link opens in new tab", content);
            Assert.Single(content.Split("opens in", StringSplitOptions.RemoveEmptyEntries));
        }

        [Fact]
        public void Process_ShowNewTabTextDefaultValue_IsTrue()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper();

            // Act & Assert
            Assert.True(tagHelper.ShowNewTabText);
        }

        [Fact]
        public void Process_WithWhitespaceOnlyHref_SuppressesOutput()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "   "
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.True(output.IsContentModified);
            Assert.True(output.TagName == null);
        }

        [Theory]
        [InlineData("https://example.com")]
        [InlineData("http://example.com")]
        [InlineData("mailto:test@example.com")]
        [InlineData("/relative/path")]
        public void Process_WithVariousValidHrefs_CreatesAnchorTag(string href)
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = href
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("a", output.TagName);
            Assert.Equal(href, output.Attributes["href"].Value);
        }

        [Fact]
        public void Process_WithEmptyContent_AddsOnlyNewTabText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true
            };
            var context = CreateTagHelperContext();
            var output = new TagHelperOutput(
                "external-link",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent().SetContent("")));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal(" (opens in new tab)", output.Content.GetContent());
        }

        [Fact]
        public void Process_WithNullScreenReaderDescriptionAndShowNewTabTextTrue_UsesDefaultText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true,
                ScreenReaderDescription = null
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Contains("(opens in new tab)", output.Content.GetContent());
        }

        [Fact]
        public void Process_WithEmptyScreenReaderDescriptionAndShowNewTabTextTrue_UsesDefaultText()
        {
            // Arrange
            var tagHelper = new ExternalLinkTagHelper
            {
                Href = "https://example.com",
                ShowNewTabText = true,
                ScreenReaderDescription = ""
            };
            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Contains("(opens in new tab)", output.Content.GetContent());
        }
    }
}