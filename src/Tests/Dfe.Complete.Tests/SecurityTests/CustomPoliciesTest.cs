using System.Security.Claims;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Infrastructure.Security.Authorization;
using Dfe.Complete.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Tests.SecurityTests;

public class FakeAuthenticateResultFeature : IAuthenticateResultFeature
{
    public AuthenticateResult? AuthenticateResult { get; set; } = AuthenticateResult.NoResult();
}

public class CustomPoliciesIntegrationTests
{
    private static AuthorizationOptions BuildAuthorizationOptions()
    {
        var options = new AuthorizationOptions();
        foreach (var policyCustomization in CustomPolicies.PolicyCustomizations)
        {
            options.AddPolicy(policyCustomization.Key, builder =>
            {
                policyCustomization.Value(builder);
            });
        }
        return options;
    }

    private static DefaultHttpContext CreateHttpContext(ClaimsPrincipal user)
    {
        var context = new DefaultHttpContext
        {
            User = user
        };
        var ticket = new AuthenticationTicket(user, "TestScheme");
        var authResult = AuthenticateResult.Success(ticket);
        context.Features.Set<IAuthenticateResultFeature>(new FakeAuthenticateResultFeature { AuthenticateResult = authResult });
        return context;
    }

    private static DefaultAuthorizationPolicyProvider CreatePolicyProvider()
    {
        var options = Options.Create(BuildAuthorizationOptions());
        return new DefaultAuthorizationPolicyProvider(options);
    }

    public static readonly object[][] HasPolicyTestData = {
        ["CanViewTeamProjectsUnassigned", new[] { "service_support" }, false],
        ["CanViewTeamProjectsUnassigned", new[] { "service_support", "manage_team" }, false],
        ["CanViewTeamProjectsUnassigned", new[] { "regional_casework_services" }, false],
        ["CanViewTeamProjectsUnassigned", new[] { "regional_casework_services", "manage_team" }, true],
        ["CanViewTeamProjectsUnassigned", new[] { "regional_delivery_officer" }, false],
        ["CanViewTeamProjectsUnassigned", new[] { "regional_delivery_officer", "manage_team" }, true],
        ["CanCreateProjects", new[] { "service_support" }, false],
        ["CanCreateProjects", new[] { "service_support", "manage_team" }, false],
        ["CanCreateProjects", new[] { "regional_casework_services" }, true],
        ["CanCreateProjects", new[] { "regional_casework_services", "manage_team" }, false],
        ["CanCreateProjects", new[] { "regional_delivery_officer" }, true],
        ["CanCreateProjects", new[] { "regional_delivery_officer", "manage_team" }, true],
        ["ActiveUser", new string[] { }, false],
        ["ActiveUser", new[] { "some_role" }, false],
        ["ActiveUser", new[] { "some_role" }, true, true] // true for hasUserId parameter
    };

    [Theory]
    [MemberData(nameof(HasPolicyTestData))]
    public async Task HasPolicy_EvaluatesCorrectly(string policyName, string[] roles, bool expectedOutcome, bool hasUserId = false)
    {
        // Arrange
        var policyProvider = CreatePolicyProvider();
        var policy = await policyProvider.GetPolicyAsync(policyName);
        Assert.NotNull(policy);

        var identity = new ClaimsIdentity("TestScheme");
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        identity.AddClaim(new Claim("sub", "test-user"));

        if (hasUserId)
        {
            identity.AddClaim(new Claim(CustomClaimTypeConstants.UserId, "test-user-id"));
        }
        
        var user = new ClaimsPrincipal(identity);

        _ = CreateHttpContext(user);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options =>
        {
            foreach (var policyCustomization in CustomPolicies.PolicyCustomizations)
            {
                options.AddPolicy(policyCustomization.Key, builder =>
                {
                    policyCustomization.Value(builder);
                });
            }
        });
        
        // Register the ActiveUserAuthorizationHandler for ActiveUser policy tests
        services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();
        
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act
        var result = await authorizationService.AuthorizeAsync(user, resource: null, policy);

        // Assert
        Assert.Equal(expectedOutcome, result.Succeeded);
    }

    [Fact]
    public async Task ActiveUserPolicy_RequiresUserIdClaim()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(UserPolicyConstants.ActiveUser, builder =>
            {
                builder.RequireAuthenticatedUser();
                builder.AddRequirements(new ActiveUserRequirement());
            });
        });
        services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();
        
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Test user without UserId claim
        var identityWithoutUserId = new ClaimsIdentity("TestScheme");
        identityWithoutUserId.AddClaim(new Claim("sub", "test-user"));
        identityWithoutUserId.AddClaim(new Claim(ClaimTypes.Role, "some_role"));
        var userWithoutUserId = new ClaimsPrincipal(identityWithoutUserId);

        // Test user with UserId claim
        var identityWithUserId = new ClaimsIdentity("TestScheme");
        identityWithUserId.AddClaim(new Claim("sub", "test-user"));
        identityWithUserId.AddClaim(new Claim(ClaimTypes.Role, "some_role"));
        identityWithUserId.AddClaim(new Claim(CustomClaimTypeConstants.UserId, "test-user-id"));
        var userWithUserId = new ClaimsPrincipal(identityWithUserId);

        // Act
        var resultWithoutUserId = await authorizationService.AuthorizeAsync(userWithoutUserId, UserPolicyConstants.ActiveUser);
        var resultWithUserId = await authorizationService.AuthorizeAsync(userWithUserId, UserPolicyConstants.ActiveUser);

        // Assert
        Assert.False(resultWithoutUserId.Succeeded, "User without UserId claim should be denied access");
        Assert.True(resultWithUserId.Succeeded, "User with UserId claim should be granted access");
    }
}
