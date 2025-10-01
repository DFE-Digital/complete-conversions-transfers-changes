using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Application.Projects.Commands.TaskData;

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
        /// Gets the Conversion tasks data by Id.
        /// </summary>
        /// <param name="request">The Conversion tasks data Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("TaskData/Conversion")]
        [SwaggerResponse(200, "Conversion tasks data returned successfully.", typeof(ConversionTaskDataDto))]
        [SwaggerResponse(404, "Conversion tasks data not found for the given Id.")]
        public async Task<IActionResult> GetConversionTasksDataByIdAsync([FromQuery] GetConversionTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            var conversionTasksData = await sender.Send(request, cancellationToken);

            return Ok(conversionTasksData.Value);
        }
        /// <summary>
        /// Updates the handover with delivery officer for either conversion or transfer tasks data.
        /// </summary>
        /// <param name="request">The update task data command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/HandoverDeliveryOfficer")]
        [SwaggerResponse(204, "Successfully updated the conversion or trasnfer task data")]
        [SwaggerResponse(404, "Transfer or Conversion task data not found for the given task data Id.")]
        public async Task<IActionResult> UpdateHandoverWithDeliveryOfficerTaskDataByTaskDataIdAsync([FromBody] UpdateHandoverWithDeliveryOfficerTaskCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the article of association task Data for conversion or trasnfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPatch]
        [Route("TaskData/ArticleOfAssociation")]
        [SwaggerResponse(204, "Conversion or transfer's article of association task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateArticleOfAssociationTaskAsync(
            [FromBody] UpdateArticleOfAssociationTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the deed of variation task Data for conversion or trasnfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/DeedOfNovationAndVariation")]
        [SwaggerResponse(204, "Transfer's deed of variation task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateDeedOfNovationAndVariationTaskAsync(
            [FromBody] UpdateDeedOfNovationAndVariationTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Updates the deed of variation task Data for conversion or trasnfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/DeedOfVariation")]
        [SwaggerResponse(204, "Conversion or transfer's deed of variation task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateDeedOfVariationTaskAsync(
            [FromBody] UpdateDeedOfVariationTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the External stakeholder kickoff Task Data for a specific task data.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPatch("TaskData/ExternalStakeholderKickoff")]
        [SwaggerResponse(204, "External stakeholder kickoff task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateExternalStakeholderKickOffTaskAsync(
            [FromBody] UpdateExternalStakeholderKickOffTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the supplemental funding agreement task Data for conversion or trasnfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/SupplementalFundingAgreement")]
        [SwaggerResponse(204, "Conversion or transfer's supplemental funding agreement task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateSupplementalFundingAgreementTaskAsync(
            [FromBody] UpdateSupplementalFundingAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        } 

        /// <summary>
        /// Updates the redact and send documents task Data for conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/RedactAndSendDocuments")]
        [SwaggerResponse(204, "Conversion or transfer's redact and send documents task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateRedactAndSendDocumentsTaskAsync(
            [FromBody] UpdateRedactAndSendDocumentsTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Updates the confirm proposed capacity of the academy for a specific task data.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch("TaskData/ConfirmProposedAcademyCapacity")]
        [SwaggerResponse(204, "Confirm proposed capacity of the academy task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateConfirmProposedCapacityOfTheAcademyTaskAsync(
            [FromBody] UpdateConfirmProposedCapacityOfTheAcademyTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Updates the receive declaration of expenditure certificate task Data for conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ReceiveDeclarationOfExpenditureCertificate")]
        [SwaggerResponse(204, "Conversion or transfer's receive declaration of expenditure certificate task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateDeclarationOfExpenditureCertificateTaskAsync(
            [FromBody] UpdateDeclarationOfExpenditureCertificateTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Confirm transfer project has authority to proceed task updated successfully.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmTransferHasAuthorityToProceed")]
        [SwaggerResponse(204, "Confirm transfer project has authority to proceed task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateConfirmTransferHasAuthorityToProceedTaskAsync(
            [FromBody] UpdateConfirmTransferHasAuthorityToProceedTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Updates the mastre funding agreement task Data for conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/MasterFundingAgreement")]
        [SwaggerResponse(204, "Conversion or transfer's master funding agreement task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateMasterFundingAgreementTaskAsync(
            [FromBody] UpdateMasterFundingAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
    }
}
