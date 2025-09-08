using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Infrastructure.Security.Authorization;

public class ActiveUserRequirement : IAuthorizationRequirement
{
    // This requirement indicates that the user must exist and be active in the database
}
