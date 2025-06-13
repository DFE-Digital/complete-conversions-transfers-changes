using Dfe.Complete.Validators;
using DfE.CoreLibs.Security.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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

        private static IConfigurationRoot GetConfiguration(string headerKey, string headerVaule)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("RequestHeaderKey", headerKey),
                    new KeyValuePair<string, string>("RequestHeaderValue", headerVaule)
                }!)
                .Build();
            return configuration;
        }

        [Fact]
        public void IsCustomRequest_ReturnsFalse_WhenNoHeaderKey()
        {
            // Arrange   
            var httpContext = CreateHttpContext("ruby");
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var checker = new HasHeaderKeyExistsInRequestValidator(configuration);

            // Act  
            var result = checker.IsValidRequest(httpContext);

            // Assert  
            Assert.Equal(OperatorType.And, checker.Operator);
            Assert.False(result);
        }

        [Fact]
        public void IsCustomRequest_ReturnsFalse_WhenHeaderKeyDoesNotMatched()
        {
            // Arrange  
            var httpContext = CreateHttpContext("ruby");
            var configuration = GetConfiguration("x-header-key", "dotnet");
            var checker = new HasHeaderKeyExistsInRequestValidator(configuration);

            // Act  
            var result = checker.IsValidRequest(httpContext);

            // Assert  
            Assert.Equal(OperatorType.And, checker.Operator);
            Assert.False(result);
        }

        [Fact]
        public void IsCustomRequest_ReturnsTrue_WhenHeaderKeyMatchesButNotValue()
        {
            // Arrange  
            var httpContext = CreateHttpContext("ruby");
            var configuration = GetConfiguration(HeaderKey, "dotnet");
            var checker = new HasHeaderKeyExistsInRequestValidator(configuration);

            // Act  
            var result = checker.IsValidRequest(httpContext);

            // Assert  
            Assert.Equal(OperatorType.And, checker.Operator);
            Assert.False(result);
        }

        [Fact]
        public void IsCustomRequest_ReturnsTrue_WhenBothHeaderKeyAndValueMatches()
        {
            // Arrange  
            var httpContext = CreateHttpContext("ruby");
            var configuration = GetConfiguration(HeaderKey, "ruby");
            var checker = new HasHeaderKeyExistsInRequestValidator(configuration);

            // Act  
            var result = checker.IsValidRequest(httpContext);

            // Assert  
            Assert.Equal(OperatorType.And, checker.Operator);
            Assert.True(result);
        }
    }
}
