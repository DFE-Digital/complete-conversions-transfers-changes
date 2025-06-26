using Dfe.Complete.Application.Users.Queries.SearchUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Controllers;

[Route("Search/User")]
[Authorize]
public class UserSearchController(ISender sender) : Controller
{
    // GET
    public async Task<IActionResult> Index(string query, string? type)
    {
        var userQuery = new SearchUsersQuery(query, type?.ToLowerInvariant() == "assignable");
        var users = await sender.Send(userQuery);
        if (users.Value is null)
        {
            return Ok();
        }

        var formattedArray = users.Value.Select(user => new[] { user.FirstName, user.LastName, user.Email }).ToArray();
        
        return Ok(formattedArray);
    }
}