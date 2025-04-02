using System.Security.Claims;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserAdId(this ClaimsPrincipal value)
        {
            var userAdId = value.Claims.SingleOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;

            return userAdId;
        }
        
        public static async Task<UserDto> GetUser(this ClaimsPrincipal value, ISender sender)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
