using Dfe.Complete.Extensions;

namespace Dfe.Complete.Tests.Extensions
{

    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("PascalCaseString", "Pascal Case String")]
        [InlineData("AnotherExampleHere", "Another Example Here")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void SplitPascalCase_ShouldSplitPascalCaseCorrectly(string input, string expected)
        {
            var result = input.SplitPascalCase();
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData("hello world", "Hello world")]
        [InlineData("HELLO", "Hello")]
        [InlineData("h", "H")]
        [InlineData("", "")]
        public void SentenceCase_ShouldConvertToSentenceCase(string input, string expected)
        {
            var result = input.SentenceCase();
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData("hello world", "Hello World")]
        [InlineData("HELLO WORLD", "Hello World")]
        [InlineData("h", "H")]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        public void ToCamelCase_ShouldConvertToCamelCase(string input, string expected)
        {
            var result = input.ToCamelCase();
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData("0", 0)]
        [InlineData("abc", 0)]
        [InlineData("", 0)]
        [InlineData(null, 0)]
        public void ToInt_ShouldConvertStringToIntOrReturnZero(string input, int expected)
        {
            var result = input.ToInt();
            Assert.Equal(result, expected);
        }
    }

}
