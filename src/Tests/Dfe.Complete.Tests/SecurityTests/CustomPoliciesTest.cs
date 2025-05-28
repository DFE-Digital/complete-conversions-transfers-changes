using System.Security.Claims;
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
        ["CanViewTeamProjectsUnassigned", new string[] { "service_support" }, false],
        ["CanViewTeamProjectsUnassigned", new string[] { "service_support", "manage_team" }, false],
        ["CanViewTeamProjectsUnassigned", new string[] { "regional_casework_services" }, false],
        ["CanViewTeamProjectsUnassigned", new string[] { "regional_casework_services", "manage_team" }, true],
        ["CanViewTeamProjectsUnassigned", new string[] { "regional_delivery_officer" }, false],
        ["CanViewTeamProjectsUnassigned", new string[] { "regional_delivery_officer", "manage_team" }, true],
        ["CanViewYourProjects", new string[] { "service_support" }, false],
        ["CanViewYourProjects", new string[] { "service_support", "manage_team" }, false],
        ["CanViewYourProjects", new string[] { "regional_casework_services" }, true],
        ["CanViewYourProjects", new string[] { "regional_casework_services", "manage_team" }, false],
        ["CanViewYourProjects", new string[] { "regional_delivery_officer" }, true],
        ["CanViewYourProjects", new string[] { "regional_delivery_officer", "manage_team" }, true],
        ["CanCreateProjects", new string[] { "service_support" }, false],
        ["CanCreateProjects", new string[] { "service_support", "manage_team" }, false],
        ["CanCreateProjects", new string[] { "regional_casework_services" }, true],
        ["CanCreateProjects", new string[] { "regional_casework_services", "manage_team" }, false],
        ["CanCreateProjects", new string[] { "regional_delivery_officer" }, true],
        ["CanCreateProjects", new string[] { "regional_delivery_officer", "manage_team" }, true]
    };

    [Theory]
    [MemberData(nameof(HasPolicyTestData))]
    public async Task HasPolicy_EvaluatesCorrectly(string policyName, string[] roles, bool expectedOutcome)
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
        var user = new ClaimsPrincipal(identity);

        var context = CreateHttpContext(user);
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
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act
        var result = await authorizationService.AuthorizeAsync(user, resource: null, policy);

        // Assert
        Assert.Equal(expectedOutcome, result.Succeeded);
    }
}
