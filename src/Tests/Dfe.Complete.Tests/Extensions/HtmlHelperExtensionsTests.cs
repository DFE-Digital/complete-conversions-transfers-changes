using Dfe.Complete.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace Dfe.Complete.Tests.Extensions
{
    public class HtmlHelperExtensionsTests
    {
        [Fact]
        public void SimpleFormat_FormatsParagraphsAndSpacesCorrectly()
        {
            // Arrange
            var htmlHelperMock = new Mock<IHtmlHelper>();
            var htmlHelper = htmlHelperMock.Object;

            string input = @"THomas McInnes

SPACE


MORE SPACE
      test inline space";

            // Act
            var result = htmlHelper.SimpleFormat(input);

            // Assert
            var expected =
                "<p>THomas McInnes</p>\n" +
                "<p>SPACE</p>\n" +
                "<p>MORE SPACE<br />\n      test inline space</p>";

            Assert.Equal(expected, result.ToString());
        }
    }
}
