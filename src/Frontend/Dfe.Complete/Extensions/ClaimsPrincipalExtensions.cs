using System.Security.Claims;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserAdId(this ClaimsPrincipal value)
        {
            var userAdId = value.Claims.SingleOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
            return userAdId ?? throw new InvalidOperationException("User does not have an objectidentifier claim.");
        }

        public static async Task<ProjectTeam> GetUserTeam(this ClaimsPrincipal value, ISender sender)
        {
            var userQuery = new GetUserByAdIdQuery(value.GetUserAdId());
            var userResponse = (await sender.Send(userQuery))?.Value;
            return EnumExtensions.FromDescription<ProjectTeam>(userResponse?.Team);
        }

        public static bool HasRole(this ClaimsPrincipal user, string role)
        {
            return user?.Claims.Any(c =>
                c.Type == ClaimTypes.Role && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        public static async Task<UserDto> GetUser(this ClaimsPrincipal value, ISender sender)
        {
            var userAdId = value.GetUserAdId();

            var request = new GetUserByAdIdQuery(userAdId);
            var userResult = await sender.Send(request);

            if (!userResult.IsSuccess || userResult.Value == null)
            {
                throw new NotFoundException(userResult.Error ?? "User not found.");
            }

            return userResult.Value;
        }
    }
}
