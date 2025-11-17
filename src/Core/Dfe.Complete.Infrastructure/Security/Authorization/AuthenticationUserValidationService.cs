using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Dfe.Complete.Infrastructure.Security.Authorization;

public interface IAuthenticationUserValidationService
{
    /// <summary>
    /// Validates and updates user information based on authentication claims
    /// </summary>
    /// <param name="principal">The authenticated user's claims principal</param>
    /// <returns>True if validation successful, false if user should be rejected</returns>
    Task<bool> ValidateUserOnAuthenticationAsync(ClaimsPrincipal principal);
}

public class AuthenticationUserValidationService(ICompleteRepository<User> userRepository, IMemoryCache cache, ILogger<AuthenticationUserValidationService> logger) 
    : IAuthenticationUserValidationService
{
    public async Task<bool> ValidateUserOnAuthenticationAsync(ClaimsPrincipal principal)
    {
            var userId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return [];

            string cacheKey = $"UserClaims_{userId}";

            if (!cache.TryGetValue(cacheKey, out List<Claim>? additionalClaims))
            {
                // Try to find user by OID first
                var userRecord = await userRepository.FindAsync(u => u.EntraUserObjectId == userId);
                var email = principal.FindFirst(CustomClaimTypeConstants.PreferredUsername)?.Value;

                // If the email doesn't match (claims vs DB) - reject.
                // Persons name probably changed and there is probably an orphaned record. Contact service support
                if (userRecord != null && !string.Equals(userRecord.Email, email, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Duplicate account detected. Your current email ({email}) doesn't match the email associated with your account. This may indicate a duplicate account. Please contact service support");

                // If there was no OID match but there was an email match, this is probably first login. 
                if (userRecord == null && !string.IsNullOrEmpty(email))
                {
                    userRecord = await userRepository.FindAsync(u => u.Email == email);
                    if (userRecord != null)
                    {
                        userRecord.EntraUserObjectId = userId;
                        await userRepository.UpdateAsync(userRecord);
                    }
                }

                // If no OID or email match, reject
                if (userRecord == null)
                    return [];
            }
    }
        // try
        // {
        //     var userId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         logger.LogWarning("No user ID found in authentication claims");
        //         return false;
        //     }

        //     var command = new ValidateAndRetrieveUserCommand(principal, userId);
        //     var result = await sender.Send(command);

        //     if (!result.IsSuccess)
        //     {
        //         logger.LogWarning("User validation failed for user ID {UserId}: {Error}", userId, result.Error);
        //         return false;
        //     }

        //     var user = result.Value;
        //     if (user == null)
        //     {
        //         logger.LogWarning("User not found in system for user ID {UserId}", userId);
        //         return false;
        //     }

        //     logger.LogInformation("User validation successful for user ID {UserId}", userId);
        //     return true;
        // }
        // catch (Exception ex)
        // {
        //     logger.LogError(ex, "Error during user validation on authentication");
        //     return false;
        // }
    }
}
