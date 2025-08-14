using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ConversionTasksDataController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets the Conversion tasks data by Id.
        /// </summary>
        /// <param name="request">The Conversion tasks data Id.</param>
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
    }
}
