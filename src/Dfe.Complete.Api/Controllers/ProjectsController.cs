using Asp.Versioning;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProjectsController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Creates a new Project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //[Authorize(Policy = "API.Write")]
        [HttpPost]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateProject_Async([FromBody] CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }


    }
}
