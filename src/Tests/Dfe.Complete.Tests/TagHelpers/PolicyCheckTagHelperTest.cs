using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;

namespace Dfe.Complete.TagHelpers.Tests
{
    public class PolicyCheckTagHelperTest
    {
        private static TagHelperContext CreateTagHelperContext() =>
            new(
                tagName: "div",
                allAttributes: [],
                items: new Dictionary<object, object>(),
                uniqueId: "test");

        private static TagHelperOutput CreateTagHelperOutput() =>
            new(
                "div",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        [Fact]
        public async Task ProcessAsync_UserIsNull_SuppressesOutput()
        {
            // Arrange
            var authorizationService = Substitute.For<IAuthorizationService>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns((HttpContext)null);

            var tagHelper = new PolicyCheckTagHelper(authorizationService, httpContextAccessor)
            {
                Policy = "TestPolicy"
            };

            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.Null(output.TagName);
        }

        [Fact]
        public async Task ProcessAsync_AuthorizationFails_SuppressesOutput()
        {
            // Arrange
            var authorizationResult = AuthorizationResult.Failed();
            var authorizationService = Substitute.For<IAuthorizationService>();
            authorizationService
                .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<string>())
                .Returns(Task.FromResult(authorizationResult));

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("TestAuthentication"));
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            var tagHelper = new PolicyCheckTagHelper(authorizationService, httpContextAccessor)
            {
                Policy = "TestPolicy"
            };

            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.Null(output.TagName);
        }

        [Fact]
        public async Task ProcessAsync_AuthorizationSucceeds_DoesNotSuppressOutput()
        {
            // Arrange
            var authorizationResult = AuthorizationResult.Success();
            var authorizationService = Substitute.For<IAuthorizationService>();
            authorizationService
                .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<string>())
                .Returns(Task.FromResult(authorizationResult));

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("TestAuthentication"));
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            var tagHelper = new PolicyCheckTagHelper(authorizationService, httpContextAccessor)
            {
                Policy = "TestPolicy"
            };

            var context = CreateTagHelperContext();
            var output = CreateTagHelperOutput();

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.NotNull(output.TagName);
            Assert.Equal("div", output.TagName);
        }
    }
}