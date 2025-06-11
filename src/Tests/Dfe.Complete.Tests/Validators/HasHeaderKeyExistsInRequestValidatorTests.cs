using Dfe.Complete.Validators;
using Microsoft.AspNetCore.Http; 

namespace Dfe.Complete.Tests.Validators
{
    public class HasHeaderKeyExistsInRequestValidatorTests
    {
        private static readonly string HeaderKey = "x-custom-request";

        private static DefaultHttpContext CreateHttpContext(string headerValue)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderKey] = headerValue;
            return httpContext;
        }

        [Fact]
        public void IsCustomRequest_ReturnsFalse_WhenNoHeaderKey()
        {
            // Arrange 

            var httpContext = CreateHttpContext("ruby");
            var checker = new HasHeaderKeyExistsInRequestValidator();

            // Act
            var result = checker.IsValidRequest(httpContext, null);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void IsCustomRequest_ReturnsFalse_WhenHeaderKeyDoesNotMatched()
        {
            // Arrange 

            var httpContext = CreateHttpContext("ruby");
            var checker = new HasHeaderKeyExistsInRequestValidator();

            // Act
            var result = checker.IsValidRequest(httpContext, "x-header-key");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCustomRequest_ReturnsTrue_WhenHeaderKeyMatches()
        {
            // Arrange 

            var httpContext = CreateHttpContext("ruby");
            var checker = new HasHeaderKeyExistsInRequestValidator();

            // Act
            var result = checker.IsValidRequest(httpContext, HeaderKey);

            // Assert
            Assert.True(result);
        }
    }
}
