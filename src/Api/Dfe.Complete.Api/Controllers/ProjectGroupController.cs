using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Queries.GetProject;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProjectGroupController(ISender sender) : ControllerBase
    {
        ///// <summary>
        ///// Gets the Project group by Id.
        ///// </summary>
        ///// <param name="id">The group Id.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        //[Authorize(Policy = "CanRead")]
        //[HttpGet("/{id:guid}")]
        //[SwaggerResponse(200, "Project Group returned successfully.", typeof(ProjectGroupDto))]
        //[SwaggerResponse(404, "Project Group not found for the given Id.")]
        //public async Task<IActionResult> GetProjectGroupByIdAsync(Guid id, CancellationToken cancellationToken)
        //{
        //    var groupId = new ProjectGroupId(id);

        //    var request = new GetProjectGroupByIdQuery(groupId);
        //    var projectGroup = await sender.Send(request, cancellationToken);

        //    return Ok(projectGroup);
        //}

        /// <summary>
        /// Gets the Project group by Id.
        /// </summary>
        /// <param name="request">The group Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [SwaggerResponse(200, "Project Group returned successfully.", typeof(ProjectGroupDto))]
        [SwaggerResponse(404, "Project Group not found for the given Id.")]
        public async Task<IActionResult> GetProjectGroupByIdAsync([FromQuery] GetProjectGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var projectGroup = await sender.Send(request, cancellationToken);

            return Ok(projectGroup);
        }
    }
}
