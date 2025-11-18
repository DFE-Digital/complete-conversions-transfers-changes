using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Security.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;

using OidcContext = Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext;

namespace Dfe.Complete.Infrastructure.Security.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services,
                IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            services.AddApplicationAuthorization(configuration);

            return services;
        }

        /// <summary>
        /// Configures OpenIdConnect events for web authentication to handle user validation
        /// </summary>
        public static IServiceCollection AddWebAuthenticationWithUserValidation(this IServiceCollection services)
        {
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Events ??= new OpenIdConnectEvents();

                var originalOnTokenValidated = options.Events.OnTokenValidated;

                options.Events.OnTokenValidated = async context =>
                {
                    // Call the original event if it exists
                    if (originalOnTokenValidated != null)
                        await originalOnTokenValidated(context);

                    if (context.Principal == null)
                    {
                        HandleSigninFailure(context, AuthenticationValidationFailure.NoPrincipal);
                        return;
                    }

                    await ValidateUserAsync(context);
                };
            });

            return services;
        }

        private static async Task ValidateUserAsync(OidcContext context)
        {
            // 1. Get users OID from claims
            var userId = context.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var email = context.Principal?.FindFirst(CustomClaimTypeConstants.PreferredUsername)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                HandleSigninFailure(context, AuthenticationValidationFailure.NoEmail);
                return;
            }

            try
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<CompleteContext>();
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<OpenIdConnectEvents>>();

                // Check if user exists by OID first
                if (!string.IsNullOrEmpty(userId) && await ValidateUserByOidAsync(dbContext, logger, userId, email, context))
                {
                    return;
                }

                // If not found by OID, check by email
                await ValidateUserByEmailAsync(dbContext, logger, userId, email, context);
            }
            catch (Exception ex)
            {
                HandleSigninFailure(context, AuthenticationValidationFailure.ValidationFailed);
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<OpenIdConnectEvents>>();
                logger.LogError(ex, "Error during user validation for user {UserId}", userId);
                context.HttpContext.Response.Redirect("/sign-in?error=validation_failed");
                context.HandleResponse();
            }
        }

        private static async Task<bool> ValidateUserByOidAsync(CompleteContext dbContext, ILogger logger,
            string userId, string email, OidcContext context)
        {
            // 2. Check if they have a matching DB record by OID
            var userByOid = await dbContext.Users.FirstOrDefaultAsync(u => u.EntraUserObjectId == userId);

            if (userByOid == null)
                return false;

            // 3. If they have a matching DB record but the email doesn't match, send error
            if (!string.Equals(userByOid.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Email mismatch for user {UserId}. DB email: {DbEmail}, Claims email: {ClaimsEmail}",
                    userId, userByOid.Email, email);
                HandleSigninFailure(context, AuthenticationValidationFailure.DuplicateAccount);
                return true;
            }

            // 4. If the email does match, allow the user into the system. Hooray!
            logger.LogInformation("User {UserId} authenticated successfully", userId);
            return true;
        }

        private static async Task ValidateUserByEmailAsync(CompleteContext dbContext, ILogger logger,
            string? userId, string email, OidcContext context)
        {
            // 5. If the user record isn't found using OID, read the email from claims
            var userByEmail = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (userByEmail == null)
            {
                // 6. If that's not found, reject with account not found
                logger.LogWarning("No user found with email {Email}", email);
                HandleSigninFailure(context, AuthenticationValidationFailure.UserNotFound);
                return;
            }

            // 7. If it is found but the OID doesn't match, flag an error
            if (!string.IsNullOrEmpty(userByEmail.EntraUserObjectId) && userByEmail.EntraUserObjectId != userId)
            {
                logger.LogError("Email {Email} is registered to a different user. DB OID: {DbOid}, Claims OID: {ClaimsOid}",
                    email, userByEmail.EntraUserObjectId, userId);
                HandleSigninFailure(context, AuthenticationValidationFailure.EmailConflict);
                return;
            }

            // 8. If the OID is empty, update it
            if (string.IsNullOrEmpty(userByEmail.EntraUserObjectId) && !string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("Updating user {Email} with Entra Object ID {OID} for first login", email, userId);
                userByEmail.EntraUserObjectId = userId;
                userByEmail.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }

            // 9. Let the user in
            logger.LogInformation("User {Email} authenticated successfully", email);
        }

        private static void HandleSigninFailure(OidcContext context, AuthenticationValidationFailure validationFailure)
        {
            context.HttpContext.Response.Redirect($"/sign-in?error={validationFailure.ToDescription()}");
            context.HandleResponse();
        }
    }
}
