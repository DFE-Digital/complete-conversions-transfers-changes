using Asp.Versioning;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Queries.GetProject;

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
        public async Task<IActionResult> CreateProject_Async([FromBody] CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }
        
        /// <summary>
        /// Gets a Project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //[Authorize(Policy = "API.Read")]
        [HttpGet]
        [SwaggerResponse(200, "Project", typeof(Project))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetProject_Async([FromBody] GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project);
        }

    }
}
