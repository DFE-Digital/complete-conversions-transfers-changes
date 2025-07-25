using Asp.Versioning;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ConversionTasksDataController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets the conversion tasks data by Id.
        /// </summary>
        /// <param name="request">The conversion tasks data Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [SwaggerResponse(200, "Conversion tasks data returned successfully.", typeof(ConversionTaskDataDto))]
        [SwaggerResponse(404, "Conversion tasks data not found for the given Id.")]
        public async Task<IActionResult> GetConversionTasksDataByIdAsync([FromQuery] GetConversionTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            var conversionTasksData = await sender.Send(request, cancellationToken);

            return Ok(conversionTasksData.Value);
        }
        
        /// <summary>
        /// Updates the External stakeholder kickoff Task Data for a specific task data.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPatch("external-stakeholder-kickoff")]
        [SwaggerResponse(204, "External stakeholder kickoff Task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateExternalStakeholderKickOffTaskAsync(
            [FromBody] UpdateExternalStakeholderKickOffTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
    }
}
