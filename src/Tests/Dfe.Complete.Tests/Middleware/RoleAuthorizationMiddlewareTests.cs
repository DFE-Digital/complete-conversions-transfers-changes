using Dfe.Complete.Configuration;
using Dfe.Complete.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Dfe.Complete.Tests.Middleware;

public class RoleAuthorizationMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<RoleAuthorizationMiddleware>> _mockLogger;
    private readonly Mock<IOptions<AzureAdOptions>> _mockOptions;
    private readonly DefaultHttpContext _httpContext;

    public RoleAuthorizationMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<RoleAuthorizationMiddleware>>();
        _mockOptions = new Mock<IOptions<AzureAdOptions>>();
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_UserWithAllowedRole_CallsNext()
    {
        // Arrange
        var allowedRoles = new List<string> { "RegionalCaseworkServices", "Support.Service" };
        _mockOptions.Setup(x => x.Value).Returns(new AzureAdOptions { AllowedRoles = allowedRoles });

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "RegionalCaseworkServices"),
            new(ClaimTypes.NameIdentifier, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        _httpContext.User = new ClaimsPrincipal(identity);

        var middleware = new RoleAuthorizationMiddleware(_mockNext.Object, _mockLogger.Object, _mockOptions.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_UserWithoutAllowedRole_RedirectsToAccessDenied()
    {
        // Arrange
        var allowedRoles = new List<string> { "RegionalCaseworkServices", "Support.Service" };
        _mockOptions.Setup(x => x.Value).Returns(new AzureAdOptions { AllowedRoles = allowedRoles });

        var claims = new List<Claim>
        {
            new("roles", "UnauthorizedRole"),
            new(ClaimTypes.NameIdentifier, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        _httpContext.User = new ClaimsPrincipal(identity);
        _httpContext.Response.Headers.Clear();

        var middleware = new RoleAuthorizationMiddleware(_mockNext.Object, _mockLogger.Object, _mockOptions.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(302, _httpContext.Response.StatusCode);
        Assert.Equal("/access-denied", _httpContext.Response.Headers.Location);
        _mockNext.Verify(x => x(_httpContext), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_EndpointWithAllowAnonymous_CallsNext()
    {
        // Arrange
        var allowedRoles = new List<string> { "RegionalCaseworkServices" };
        _mockOptions.Setup(x => x.Value).Returns(new AzureAdOptions { AllowedRoles = allowedRoles });

        // Create endpoint with AllowAnonymous metadata
        var endpoint = new Endpoint(_ => Task.CompletedTask, 
            new EndpointMetadataCollection(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute()), 
            "test");
        _httpContext.SetEndpoint(endpoint);

        var middleware = new RoleAuthorizationMiddleware(_mockNext.Object, _mockLogger.Object, _mockOptions.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_UnauthenticatedUser_CallsNext()
    {
        // Arrange
        var allowedRoles = new List<string> { "RegionalCaseworkServices" };
        _mockOptions.Setup(x => x.Value).Returns(new AzureAdOptions { AllowedRoles = allowedRoles });

        // User is not authenticated
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var middleware = new RoleAuthorizationMiddleware(_mockNext.Object, _mockLogger.Object, _mockOptions.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
    }
}