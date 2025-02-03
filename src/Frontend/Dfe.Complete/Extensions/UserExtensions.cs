using System.Security.Claims;

namespace Dfe.Complete.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserAdId(this ClaimsPrincipal value)
        {
            var userAdId = value.Claims.SingleOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;

            return userAdId;
        }
    }
}
