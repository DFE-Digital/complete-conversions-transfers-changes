using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Authorization;

public class AuthorizationExtensionsTests
{
    [Fact]
    public void AddWebAuthenticationWithUserValidation_ConfiguresOpenIdConnectOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddWebAuthenticationWithUserValidation();
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnTokenValidated);
    }

    [Fact]
    public void AddWebAuthenticationWithUserValidation_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddWebAuthenticationWithUserValidation();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddWebAuthenticationWithUserValidation_CreatesEventsIfNull()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Pre-configure with null events
        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events = null!;
        });

        // Act
        services.AddWebAuthenticationWithUserValidation();
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnTokenValidated);
    }

    [Fact]
    public void AddWebAuthenticationWithUserValidation_PreservesExistingEvents()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context =>
                {
                    return Task.CompletedTask;
                }
            };
        });

        // Act
        services.AddWebAuthenticationWithUserValidation();
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert
        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnTokenValidated);
        Assert.NotNull(options.Events.OnRedirectToIdentityProvider);
    }

    [Fact]
    public async Task OnTokenValidated_NoPrincipal_RedirectsWithNoPrincipalError()
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        context.Principal = null;

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert
        context.HttpContext.Response.Received(1).Redirect("/sign-in?error=no_principal");
        Assert.True(context.Result?.Handled ?? false);
    }

    [Fact]
    public async Task OnTokenValidated_NoOidClaim_RedirectsWithNoEmailError()
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new[]
        {
                new Claim("other_claim", "value")
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert
        context.HttpContext.Response.Received(1).Redirect("/sign-in?error=no_email");
        Assert.True(context.Result?.Handled ?? false);
    }

    [Fact]
    public async Task OnTokenValidated_NoEmailClaim_RedirectsWithNoEmailError()
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new[]
        {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "user123")
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert
        context.HttpContext.Response.Received(1).Redirect("/sign-in?error=no_email");
        Assert.True(context.Result?.Handled ?? false);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task OnTokenValidated_EmptyOrNullEmail_RedirectsWithNoEmailError(string invalidEmail)
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new List<Claim>
            {
                new("http://schemas.microsoft.com/identity/claims/objectidentifier", "user123")
            };

        if (invalidEmail != null)
        {
            claims.Add(new Claim(CustomClaimTypeConstants.PreferredUsername, invalidEmail));
        }

        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert
        context.HttpContext.Response.Received(1).Redirect("/sign-in?error=no_email");
        Assert.True(context.Result?.Handled ?? false);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task OnTokenValidated_WhitespaceEmail_RedirectsWithValidationFailedError(string whitespaceEmail)
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new[]
        {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "user123"),
                new Claim(CustomClaimTypeConstants.PreferredUsername, whitespaceEmail)
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert - Whitespace emails don't trigger "no_email", they fail in DB validation
        // Due to error handling, we may get multiple redirects (one from DB failure, one from logger failure)  
        context.HttpContext.Response.Received().Redirect("/sign-in?error=validation_failed");
        Assert.True(context.Result?.Handled ?? false);
    }

    [Fact]
    public async Task OnTokenValidated_ValidEmailNoOid_ContinuesToValidation()
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new[]
        {
                new Claim(CustomClaimTypeConstants.PreferredUsername, "test@education.gov.uk")
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert - Should proceed to validation and get a database error due to mock setup
        Assert.True(context.Result?.Handled ?? false);
    }

    [Fact]
    public async Task OnTokenValidated_ServiceResolutionFails_RedirectsWithValidationFailedError()
    {
        // Arrange - Create context with no services to trigger service resolution failure
        var context = CreateMockTokenValidatedContext(setupServices: false);
        var claims = new[]
        {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "user123"),
                new Claim(CustomClaimTypeConstants.PreferredUsername, "test@education.gov.uk")
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act & Assert - Should get service resolution failure
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => InvokeTokenValidatedEvent(context));

        Assert.Contains("No service for type", exception.Message);
    }

    [Fact]
    public async Task OnTokenValidated_ValidClaimsWithMockDatabase_ContinuesToUserValidation()
    {
        // Arrange
        var context = CreateMockTokenValidatedContext();
        var claims = new[]
        {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "user123"),
                new Claim(CustomClaimTypeConstants.PreferredUsername, "test@education.gov.uk")
            };
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        await InvokeTokenValidatedEvent(context);

        // Assert - Should proceed to database validation
        // The exact error depends on mock database setup, but should not fail on claims validation
        Assert.True(context.Result?.Handled ?? false);
    }

    [Fact]
    public void AddWebAuthenticationWithUserValidation_MultipleCallsAllowed()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act - Call multiple times
        services.AddWebAuthenticationWithUserValidation();
        services.AddWebAuthenticationWithUserValidation();
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        // Assert - Should still work
        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnTokenValidated);
    }

    /// <summary>
    /// Creates a mock TokenValidatedContext for testing
    /// </summary>
    /// <param name="setupServices">Whether to setup mock services</param>
    private static TokenValidatedContext CreateMockTokenValidatedContext(bool setupServices = true)
    {
        var serviceCollection = new ServiceCollection();
        var httpContext = Substitute.For<HttpContext>();
        var response = Substitute.For<HttpResponse>();

        httpContext.Response.Returns(response);

        if (setupServices)
        {
            // Create minimal services needed for CompleteContext
            var mockConfiguration = Substitute.For<IConfiguration>();
            serviceCollection.AddSingleton(mockConfiguration);

            // Create a CompleteContext instance with minimal setup
            var mockContext = new Dfe.Complete.Infrastructure.Database.CompleteContext();
            serviceCollection.AddSingleton(mockContext);

            // Register logger with proper generic type
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<ILogger<OpenIdConnectEvents>>(provider =>
                provider.GetRequiredService<ILoggerFactory>().CreateLogger<OpenIdConnectEvents>());
        }

        var serviceProvider = serviceCollection.BuildServiceProvider();
        httpContext.RequestServices.Returns(serviceProvider);

        var authenticationScheme = new AuthenticationScheme("Test", "Test", typeof(OpenIdConnectHandler));
        var principal = new ClaimsPrincipal();
        var properties = new AuthenticationProperties();
        var context = new TokenValidatedContext(httpContext, authenticationScheme, new OpenIdConnectOptions(), principal, properties);

        return context;
    }

    /// <summary>
    /// Invokes the OnTokenValidated event configured by AddWebAuthenticationWithUserValidation
    /// </summary>
    private static async Task InvokeTokenValidatedEvent(TokenValidatedContext context)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        // Add necessary services for the authentication extension
        var mockConfiguration = Substitute.For<IConfiguration>();
        services.AddSingleton(mockConfiguration);
        services.AddSingleton<Dfe.Complete.Infrastructure.Database.CompleteContext>();
        services.AddSingleton<ILogger<OpenIdConnectEvents>>(provider =>
            provider.GetRequiredService<ILoggerFactory>().CreateLogger<OpenIdConnectEvents>());

        services.AddWebAuthenticationWithUserValidation();

        var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);

        if (options.Events?.OnTokenValidated != null)
        {
            await options.Events.OnTokenValidated(context);
        }
    }
}
