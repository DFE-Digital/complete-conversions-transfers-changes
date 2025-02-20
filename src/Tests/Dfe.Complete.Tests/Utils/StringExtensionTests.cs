using Dfe.Complete.Utils;

namespace Dfe.Complete.Tests.Utils
{
    public class StringExtensionTests
    {
        private enum TestEnumValues
        {
            A = 'A', // ASCII 65
            B = 'B', // ASCII 66
            C = 'C'  // ASCII 67
        }

        [Fact]
        public void ToEnumFromChar_ShouldReturnEnumValue_WhenInputIsValid()
        {
            // Arrange
            const string input = "A";

            // Act
            var result = input.ToEnumFromChar<TestEnumValues>();

            // Assert
            Assert.Equal(TestEnumValues.A, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ToEnumFromChar_ShouldThrowArgumentException_WhenInputIsNullOrEmpty(string input)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => input.ToEnumFromChar<TestEnumValues>());
            Assert.Equal("Input cannot be null or empty. (Parameter 'value')", ex.Message);
        }

        [Fact]
        public void ToEnumFromChar_ShouldThrowNotFoundException_WhenCharNotDefinedInEnum()
        {
            // Arrange
            const string input = "Z";

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => input.ToEnumFromChar<TestEnumValues>());
            Assert.Contains("TestEnumValues could not be found.", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Contains("'Z' (ASCII 90)", ex.InnerException.Message);
        }
    }
}