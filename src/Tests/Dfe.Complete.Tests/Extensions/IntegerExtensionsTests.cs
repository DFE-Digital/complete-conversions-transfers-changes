using Dfe.Complete.Extensions;

namespace Dfe.Complete.Tests.Extensions
{
    public class IntegerExtensionsTests
    {
        [Theory]
        [InlineData(25, 100, "25%")]
        [InlineData(1, 4, "25%")]
        [InlineData(0, 10, "0%")]
        [InlineData(null, 100, "")]
        [InlineData(50, null, "")]
        [InlineData(null, null, "")]
        public void AsPercentageOf_ReturnsExpectedResult(int? part, int? whole, string expected)
        {
            var result = part.AsPercentageOf(whole);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData("-456", -456)]
        [InlineData("0", 0)]
        [InlineData("abc", null)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void ToInt_ReturnsExpectedResult(string input, int? expected)
        {
            var result = IntegerExtensions.ToInt(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, "1 result found")]
        [InlineData(0, "0 results found")]
        [InlineData(5, "5 results found")]
        public void ToResultsCountMessage_ReturnsExpectedMessage(int value, string expected)
        {
            var result = value.ToResultsCountMessage();
            Assert.Equal(expected, result);
        }

    }
}
