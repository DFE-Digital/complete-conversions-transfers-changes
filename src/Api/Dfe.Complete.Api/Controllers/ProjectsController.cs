using Asp.Versioning;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;

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
        public async Task<IActionResult> GetProject_Async([FromQuery] GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project);
        }
        
        /// <summary>
        /// Returns a list of Projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //[Authorize(Policy = "API.Read")]
        [HttpGet]
        [Route("List/All")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjects_Async([FromQuery] ListAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project);
        }
        
        /// <summary>
        /// Returns the number of Projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //[Authorize(Policy = "API.Read")]
        [HttpGet]
        [Route("Count/All")]
        [SwaggerResponse(200, "Project", typeof(int))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CountAllProjects_Async([FromQuery] CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project);
        }

    }
}
