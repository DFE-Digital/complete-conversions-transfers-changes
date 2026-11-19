using Asp.Versioning;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.SignificantChange.Commands;
using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Queries;
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

        /// <summary>
        /// Returns a filtered, paged list of significant change projects.
        /// </summary>
        /// <param name="request">The filters and paging options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpPost]
        [Route("SignificantChange/List")]
        [SwaggerResponse(200, "Matching projects and the total count.", typeof(PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListSignificantChangeProjectsByFilterAsync([FromBody] ListSignificantChangeProjectsByFilterQuery request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        /// <summary>
        /// Returns the filter options available for the given filters.
        /// </summary>
        /// <param name="request">The filters the options are faceted against.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpPost]
        [Route("SignificantChange/FilterOptions")]
        [SwaggerResponse(200, "Available filter options.", typeof(SignificantChangeProjectFilterOptions))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetSignificantChangeProjectFilterOptionsAsync([FromBody] GetSignificantChangeProjectFilterOptionsQuery request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return Ok(result);
        }
    }
