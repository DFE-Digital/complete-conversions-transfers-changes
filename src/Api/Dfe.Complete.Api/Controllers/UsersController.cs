using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class UsersController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [SwaggerResponse(201, "User created successfully.", typeof(Guid))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);

            if (!result.IsSuccess || result.Value == null)
                return BadRequest(result.Error);
            return Created("", result.Value!.Value);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPut]
        [SwaggerResponse(204, "User updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);

            if (!response.IsSuccess)
            {
                if (response.ErrorType == ErrorType.NotFound)
                    return NotFound(response.Error);
                return StatusCode(500, response.Error);
            }


            return NoContent();
        }

        /// <summary>
        /// Gets a User with their assigned projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [SwaggerResponse(200, "Project", typeof(UserWithProjectsDto))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetUserWithProjectsAsync([FromQuery] GetUserWithProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Users with their assigned projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllUsersWithProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllUsersWithProjectsAsync([FromQuery] ListAllUsersWithProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }
    }
}
