using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Queries.TaskData;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanRead")]
    [Route("v{version:apiVersion}/[controller]")]
    public class TasksDataController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets the transfer tasks data by Id.
        /// </summary>
        /// <param name="request">The transfer tasks data Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("TaskData/Transfer")]
        [SwaggerResponse(200, "Transfer tasks data returned successfully.", typeof(TransferTaskDataDto))]
        [SwaggerResponse(404, "Transfer tasks data not found for the given Id.")]
        public async Task<IActionResult> GetTransferTasksDataByIdAsync([FromQuery] GetTransferTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            var transferTasksData = await sender.Send(request, cancellationToken);

            return Ok(transferTasksData.Value);
        }
        /// <summary>
        /// Updates the handover with delivery officer for either conversion or transfer tasks data.
        /// </summary>
        /// <param name="request">The update task data command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/Handover/DeliverOfficer")]
        [SwaggerResponse(204, "Successfully updated the conversion or trasnfer task data")]
        [SwaggerResponse(404, "Transfer or Conversion task data not found for the given project Id.")]
        public async Task<IActionResult> UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync([FromBody] UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken); 
            return NoContent();
        }

        /// <summary>
        /// Gets the Task Data by Project Id for either conversion or transfer project.
        /// </summary>
        /// <param name="request">The transfer tasks data Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("TaskData/ProjectId")]
        [SwaggerResponse(200, "Returns conversion or transfer task data.", typeof(TaskDataModel))]
        [SwaggerResponse(404, "No task data found for the given Id.")]
        public async Task<IActionResult> GetTaskDataByProjectIdAsync([FromQuery] GetTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var tasksData = await sender.Send(request, cancellationToken);

            return Ok(tasksData.Value);
        } 
    }
}
