using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using GovUK.Dfe.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Dfe.Complete.Infrastructure.Security.Authorization
{
    public class CustomDatabaseClaimsProvider(ICompleteRepository<User> userRepository, IMemoryCache cache)
        : ICustomClaimProvider
    {
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public async Task<IEnumerable<Claim>> GetClaimsAsync(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var email = principal.FindFirst(CustomClaimTypeConstants.PreferredUsername)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                return [];

            string cacheKey = $"UserClaims_{userId}";

            if (!cache.TryGetValue(cacheKey, out List<Claim>? additionalClaims))
            {
                var userRecord = await userRepository.FindAsync(u => u.EntraUserObjectId == userId && u.DeactivatedAt == null);
                if (userRecord == null || !string.Equals(userRecord.Email, email, StringComparison.OrdinalIgnoreCase))
                    return [];

                additionalClaims =
                [
                    new (CustomClaimTypeConstants.UserId, userRecord.Id.Value.ToString())
                ];

                cache.Set(cacheKey, additionalClaims, _cacheDuration);
            }

            return additionalClaims ?? [];
        }
    }
}
