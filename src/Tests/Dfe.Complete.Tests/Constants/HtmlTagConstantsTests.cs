using Dfe.Complete.Constants;

namespace Dfe.Complete.Tests.Constants
{
    public class HtmlTagConstantsTests
    {
        [Fact]
        public void Empty_ShouldReturnExpectedHtml()
        {
            var expected = "<span class=\"empty\">Empty</span>";
            Assert.Equal(HtmlTagConstants.Empty, expected);
        }
    }
}
