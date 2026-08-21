using Asp.Versioning;
using Dfe.Complete.Application.SignificantChange.Commands;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dfe.Complete.Api.Controllers;

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class SignificantChangeProjectsController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Creates a new significant change project.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("SignificantChange")]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateSignificantChangeProjectAsync([FromBody] CreateSignificantChangeProjectCommand request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return Created("", result);
        }
    }
