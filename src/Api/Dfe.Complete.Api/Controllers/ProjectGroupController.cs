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

            return Ok(projectGroup.Value);
        }
        
        /// <summary>
        /// Gets a list of the projects group and included establishments.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List")]
        [SwaggerResponse(200, "Project Groups returned successfully.", typeof(ListProjectsGroupsModel))]
        public async Task<IActionResult> GetProjectGroupsAsync([FromQuery] ListProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            var projectGroups = await sender.Send(request, cancellationToken);

            return Ok(projectGroups.Value);
        }

        /// <summary>
        /// Gets the Project group details by Id.
        /// </summary>
        /// <param name="request">The details request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("Details")]
        [SwaggerResponse(200, "Project Group details returned successfully.", typeof(ProjectGroupDetails))]
        [SwaggerResponse(404, "Project Group not found for the given Id.")]
        public async Task<IActionResult> GetProjectGroupDetailsAsync([FromQuery] GetProjectGroupDetailsQuery request, CancellationToken cancellationToken)
        {
            var projectGroupDetails = await sender.Send(request, cancellationToken);
            return Ok(projectGroupDetails.Value);
        }
    }
}
