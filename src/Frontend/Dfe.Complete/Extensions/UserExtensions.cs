using System.Security.Claims;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserAdId(this ClaimsPrincipal value)
        {
            var userAdId = value.Claims.SingleOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;

            return userAdId;
        }
        
        public static async Task<ProjectTeam> GetUserTeam(this ClaimsPrincipal value)
        {
            var userAdId = value.GetUserAdId();

            var request = new GetUserByAdIdQuery(userAdId);
            // var userResult = await sender.Send(request);
            //
            // if (userResult is { IsSuccess: false })
            // {
            //     throw new NotFoundException(userResult.Error ?? "User not found.");
            // }
            //
            var userTeam = ProjectTeam.ServiceSupport;

            // var userTeam = Enum.Parse<ProjectTeam>(userResult.Value.Team);
                
            return userTeam;
        }
    }
}
