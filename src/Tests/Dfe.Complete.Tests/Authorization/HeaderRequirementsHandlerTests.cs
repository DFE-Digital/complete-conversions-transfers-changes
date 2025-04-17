using System.Security.Claims;
using Dfe.Complete.Authorization;
using Dfe.Complete.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Net.Http.Headers;
using NSubstitute;

namespace Dfe.Complete.Tests.Authorization;

public class HeaderRequirementsHandlerTests
{
    [Theory]
    [InlineData("PREDEFINED-AD-ID", "PREDEFINED-AD-ID")]
    [InlineData(null, "TEST-AD-ID")]
    public async Task HandleRequirementAsync_WhenClientSecretValid_AddsClaimsAndSucceeds(string aDHeader,
        string expectedObjectIdentifier)
    {
        var hostEnvironment = new HostingEnvironment
        {
            EnvironmentName = Environments.Development
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append(HeaderNames.Authorization, "Bearer 123");
        httpContext.Request.Headers.Append("x-user-context-name", "John");
        httpContext.Request.Headers.Append("x-user-context-role-0", Claims.AdminRoleClaim);
        if (!string.IsNullOrEmpty(aDHeader)) httpContext.Request.Headers.Append("x-user-ad-id", aDHeader);

        var httpAccessor = Substitute.For<IHttpContextAccessor>();
        httpAccessor.HttpContext.Returns(httpContext);

        var configurationSettings = new Dictionary<string, string>
        {
            { "CypressTestSecret", "123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationSettings!)
            .Build();

        var handler = new HeaderRequirementHandler(hostEnvironment, httpAccessor, configuration);

        var requirement = Substitute.For<IAuthorizationRequirement>();
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(new ClaimsIdentity()),
            null
        );

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        var claims = context.User.Claims.ToList();
        Assert.Contains(claims, c => c is { Type: ClaimTypes.Name, Value: "John" });
        Assert.Contains(claims, c => c is { Type: ClaimTypes.Role, Value: Claims.AdminRoleClaim });
        Assert.Contains(claims, c => c.Type == "objectidentifier" && c.Value == expectedObjectIdentifier);
    }

    [Theory]
    [InlineData(null, "John", Claims.AdminRoleClaim, "x-user-ad-id")]
    [InlineData("invalid", "John", Claims.AdminRoleClaim, "x-user-ad-id")]
    [InlineData("123", null, Claims.AdminRoleClaim, "x-user-ad-id")]
    [InlineData("123", "John", "", "x-user-ad-id")]
    public async Task HandleRequirementAsync_WhenRequiredHeaderMissingOrInvalid_DoesNotSucceed(string authHeader,
        string nameHeader, string roleHeader, string adHeader)
    {
        var hostEnvironment = new HostingEnvironment
        {
            EnvironmentName = Environments.Development
        };

        var httpContext = new DefaultHttpContext();
        if (!string.IsNullOrEmpty(authHeader))
            httpContext.Request.Headers.Append(HeaderNames.Authorization, $"Bearer {authHeader}");
        if (!string.IsNullOrEmpty(nameHeader)) httpContext.Request.Headers.Append("x-user-context-name", nameHeader);
        if (!string.IsNullOrEmpty(roleHeader)) httpContext.Request.Headers.Append("x-user-context-role-0", roleHeader);
        if (!string.IsNullOrEmpty(adHeader)) httpContext.Request.Headers.Append("x-user-ad-id", adHeader);

        var httpAccessor = Substitute.For<IHttpContextAccessor>();
        httpAccessor.HttpContext.Returns(httpContext);

        var configurationSettings = new Dictionary<string, string>
        {
            { "CypressTestSecret", "123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationSettings!)
            .Build();

        var handler = new HeaderRequirementHandler(hostEnvironment, httpAccessor, configuration);

        var requirement = Substitute.For<IAuthorizationRequirement>();
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(new ClaimsIdentity()),
            null
        );

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}