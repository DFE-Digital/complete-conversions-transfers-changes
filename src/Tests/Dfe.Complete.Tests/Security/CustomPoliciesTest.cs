// using System.Security.Claims;
// using Dfe.Complete.Domain.Constants;

// namespace Dfe.Complete.Tests.Security;

// public class CustomPoliciesTest
// {
//     private static bool EvaluateAccessCanViewYourProjects(ClaimsPrincipal user)
//     {
//         if (user?.Identity is not { IsAuthenticated: true })
//             return false;

//         return user.IsInRole(UserRolesConstants.RegionalDeliveryOfficer)
//             || (user.IsInRole(UserRolesConstants.RegionalCaseworkServices) && !user.IsInRole(UserRolesConstants.ManageTeam));
//     }

//     private static bool EvaluateAccessCanViewTeamProjectsUnassigned(ClaimsPrincipal user)
//     {
//         if (user?.Identity is not { IsAuthenticated: true })
//             return false;

//         return user.IsInRole(UserRolesConstants.ManageTeam) &&
//             (user.IsInRole(UserRolesConstants.RegionalCaseworkServices) || user.IsInRole(UserRolesConstants.RegionalDeliveryOfficer));
//     }

//     public static readonly object[][] CanViewYourProjectsTestData =
//     [
//         [new[] { "service_support" }, false],
//         [new[] { "service_support", "manage_team" }, false],
//         [new[] { "regional_casework_services" }, true],
//         [new[] { "regional_casework_services", "manage_team" }, false],
//         [new[] { "regional_delivery_officer" }, true],
//         [new[] { "regional_delivery_officer", "manage_team" }, true],
//     ];

//     [Theory]
//     [MemberData(nameof(CanViewYourProjectsTestData))]
//     public void CanViewYourProjectsPolicy_EvaluatesCorrectly(string[] roles, bool expectedAccess)
//     {
//         // Arrange
//         var user = new ClaimsPrincipal(new ClaimsIdentity(
//             roles.Select(role => new Claim(ClaimTypes.Role, role)),
//             authenticationType: "TestAuthentication"));

//         // Act
//         var actualAccess = EvaluateAccessCanViewYourProjects(user);

//         // Assert
//         Assert.Equal(expectedAccess, actualAccess);
//     }

//     [Fact]
//     public void CanViewYourProjectsPolicy_UnauthenticatedUser_ShouldBeDenied()
//     {
//         // Arrange
//         var identity = new ClaimsIdentity();
//         var user = new ClaimsPrincipal(identity);

//         // Act
//         var actualAccess = EvaluateAccessCanViewYourProjects(user);

//         // Assert
//         Assert.False(actualAccess);
//     }

//     public static readonly object[][] CanViewTeamProjectsUnassignedTestData =
//     [
//         [new[] { "service_support" }, false],
//         [new[] { "service_support", "manage_team" }, false],
//         [new[] { "regional_casework_services" }, false],
//         [new[] { "regional_casework_services", "manage_team" }, true],
//         [new[] { "regional_delivery_officer" }, false],
//         [new[] { "regional_delivery_officer", "manage_team" }, true],
//     ];

//     [Theory]
//     [MemberData(nameof(CanViewTeamProjectsUnassignedTestData))]
//     public void CanViewTeamProjectsUnassignedPolicy_EvaluatesCorrectly(string[] roles, bool expectedAccess)
//     {
//         // Arrange
//         var user = new ClaimsPrincipal(new ClaimsIdentity(
//             roles.Select(role => new Claim(ClaimTypes.Role, role)),
//             authenticationType: "TestAuthentication"));

//         // Act
//         var actualAccess = EvaluateAccessCanViewTeamProjectsUnassigned(user);

//         // Assert
//         Assert.Equal(expectedAccess, actualAccess);
//     }

//     [Fact]
//     public void CanViewTeamProjectsUnassignedPolicy_UnauthenticatedUser_ShouldBeDenied()
//     {
//         // Arrange
//         var identity = new ClaimsIdentity();
//         var user = new ClaimsPrincipal(identity);

//         // Act
//         var actualAccess = EvaluateAccessCanViewTeamProjectsUnassigned(user);

//         // Assert
//         Assert.False(actualAccess);
//     }
// }



using System.Security.Claims;
using System.Threading.Tasks;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dfe.Complete.Tests.Security
{
    // A fake feature so that the evaluator sees a successful authentication.
    public class FakeAuthenticateResultFeature : IAuthenticateResultFeature
    {
        public AuthenticateResult AuthenticateResult { get; set; } = AuthenticateResult.NoResult();
    }

    public class CustomPoliciesIntegrationTests
    {
        private AuthorizationOptions BuildAuthorizationOptions()
        {
            var options = new AuthorizationOptions();
            foreach (var policyCustomization in CustomPolicies.PolicyCustomizations)
            {
                options.AddPolicy(policyCustomization.Key, builder =>
                {
                    // Call the customization to build the policy.
                    policyCustomization.Value(builder);
                });
            }
            return options;
        }

        // Creates an HttpContext with a fake successful authentication.
        private DefaultHttpContext CreateHttpContext(ClaimsPrincipal user)
        {
            var context = new DefaultHttpContext();
            context.User = user;
            // Create a fake authentication ticket so that the evaluator finds the user authenticated.
            var ticket = new AuthenticationTicket(user, "TestScheme");
            var authResult = AuthenticateResult.Success(ticket);
            context.Features.Set<IAuthenticateResultFeature>(new FakeAuthenticateResultFeature { AuthenticateResult = authResult });
            return context;
        }

        private DefaultAuthorizationPolicyProvider CreatePolicyProvider()
        {
            var options = Options.Create(BuildAuthorizationOptions());
            return new DefaultAuthorizationPolicyProvider(options);
        }

        [Theory]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "service_support" }, false)]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "service_support", "manage_team" }, false)]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "regional_casework_services" }, false)]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "regional_casework_services", "manage_team" }, true)]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "regional_delivery_officer" }, false)]
        [InlineData("CanViewTeamProjectsUnassigned", new string[] { "regional_delivery_officer", "manage_team" }, true)]
        [InlineData("CanViewYourProjects", new string[] { "service_support" }, false)]
        [InlineData("CanViewYourProjects", new string[] { "service_support", "manage_team" }, false)]
        [InlineData("CanViewYourProjects", new string[] { "regional_casework_services" }, true)]
        [InlineData("CanViewYourProjects", new string[] { "regional_casework_services", "manage_team" }, false)]
        [InlineData("CanViewYourProjects", new string[] { "regional_delivery_officer" }, true)]
        [InlineData("CanViewYourProjects", new string[] { "regional_delivery_officer", "manage_team" }, true)]
        public async Task CanViewTeamProjectsUnassignedPolicy_EvaluatesCorrectly(string policyName, string[] roles, bool expectedOutcome)
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
}