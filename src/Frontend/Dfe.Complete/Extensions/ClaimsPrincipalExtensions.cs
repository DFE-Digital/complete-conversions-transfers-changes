using System.Security.Claims;
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

            return userAdId;
        }

        public static async Task<ProjectTeam> GetUserTeam(this ClaimsPrincipal value, ISender sender)
        {
            var userQuery = new GetUserByAdIdQuery(value.GetUserAdId());
            var userResponse = (await sender.Send(userQuery))?.Value;
            return EnumExtensions.FromDescription<ProjectTeam>(userResponse?.Team);
        }
    }
}
