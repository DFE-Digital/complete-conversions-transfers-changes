using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class TransferTasksDataController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets the transfer tasks data by Id.
        /// </summary>
        /// <param name="request">The transfer tasks data Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [SwaggerResponse(200, "Transfer tasks data returned successfully.", typeof(TransferTaskDataDto))]
        [SwaggerResponse(404, "Transfer tasks data not found for the given Id.")]
        public async Task<IActionResult> GetTransferTasksDataByIdAsync([FromQuery] GetTransferTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            var transferTasksData = await sender.Send(request, cancellationToken);

            return Ok(transferTasksData.Value);
        }
    }
}
