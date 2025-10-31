using Asp.Versioning;
using Dfe.Complete.Application.KeyContacts.Commands;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmTransferHasAuthorityToProceedTaskAsync(
            [FromBody] UpdateConfirmTransferHasAuthorityToProceedTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        

        /// <summary>
        /// Confirm the date the academy transferred task updated successfully.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmDateAcademyTransferred")]
        [SwaggerResponse(204, "Confirm the date the academy transferred task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateConfirmDateAcademyTransferredTaskAsync(
            [FromBody] UpdateConfirmDateAcademyTransferredTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates confirmation of meeting all conditions met for a conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmAllConditionsMet")]
        [SwaggerResponse(204, "Confirms all conditions are met successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmAllConditionsMetTaskAsync(
            [FromBody] UpdateConfirmAllConditionsMetTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Confirm the date the academy opened for conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmAcademyOpenedDate")]
        [SwaggerResponse(204, "Confirm the date the academy opened for conversion project successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmAcademyOpenedDateTaskAsync(
            [FromBody] UpdateConfirmAcademyOpenedDateTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        

        /// <summary>
        /// Update the church supplemental agreement task Data for conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ChurchSupplementalAgreement")]
        [SwaggerResponse(204, "Conversion or transfer's church supplemental agreement task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project/User not found.")]
        public async Task<IActionResult> UpdateChurchSupplementalAgreementTaskAsync(
            [FromBody] UpdateChurchSupplementalAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>        
        /// Updates the commercial transfer agreement task Data for conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/CommercialTransferAgreement")]
        [SwaggerResponse(204, "Commercial transfer agreement task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateCommercialTransferAgreementTaskAsync(
            [FromBody] UpdateCommercialAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the main contact for either conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/MainContact")]
        [SwaggerResponse(204, "Confirms all conditions are met successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateMainContactTaskAsync(
            [FromBody] UpdateMainContactTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Confirm conversion project's land questionnaire task updated successfully.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/LandQuestionnaire")]
        [SwaggerResponse(204, "Confirm conversion project's land questionnaire task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateLandQuestionnaireTaskAsync(
            [FromBody] UpdateLandQuestionnaireTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        } 

        /// <summary>
        /// Updaing the land registry title plans task data for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/LandRegistryTitlePlans")]
        [SwaggerResponse(204, "The land registry title plans task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateLandRegistryTitlePlansTaskAsync(
            [FromBody] UpdateLandRegistryTitlePlansTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Updates the master funding agreement task Data for conversion or transfer project.
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

        /// <summary>
        /// Confirm the incoming trust ceo contact for the project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmIncomingTrustCeoContact")]
        [SwaggerResponse(204, "Confirm the incoming trust ceo contact for the project successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmIncomingTrustCeoContactTaskAsync(
            [FromBody] UpdateIncomingTrustCeoCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Confirm the academy risk protection arrangements task updated successfully for either conversion or transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmAcademyRiskProtectionArrangements")]
        [SwaggerResponse(204, "Confirm the academy risk protection arrangements task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync(
            [FromBody] UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Confirm the head teacher contact for the project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmHeadTeacherContact")]
        [SwaggerResponse(204, "Confirm the head teacher contact for the project successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmHeadTeacherContactTaskAsync(
            [FromBody] UpdateHeadTeacherCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Complete a notification of changes to funded high needs places form for the project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/CompleteNotificationOfChange")]
        [SwaggerResponse(204, "Complete a notification of changes to funded high needs places form for the project successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateCompleteNotificationOfChangeTaskAsync(
            [FromBody] UpdateCompleteNotificationOfChangeTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updating the academy and trust financial information task data for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/AcademyAndTrustFinancialInformation")]
        [SwaggerResponse(204, "Updated the academy and trust financial information task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateAcademyAndTrustFinancialInformationTaskAsync(
            [FromBody] UpdateAcademyAndTrustFinancialInformationTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updating the academy name task data for conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/AcademyDetails")]
        [SwaggerResponse(204, "Updated the academy name task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateAcademyDetailsTaskAsync(
            [FromBody] UpdateConfirmAcademyNameTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Confirm if the bank details for the general annual grant payment need to change for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmBankDetails")]
        [SwaggerResponse(204, "Confirm if the bank details for the general annual grant payment need to change task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateConfirmBankDetailsTaskAsync(
            [FromBody] UpdateConfirmBankDetailsTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updating the land consent letter task data for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/LandConsentLetter")]
        [SwaggerResponse(204, "Updated the land consent letter task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateLandConsentLetterTaskAsync(
            [FromBody] UpdateLandConsentLetterTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Check the accuracy of higher needs for conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/CheckAccuracyOfHigherNeeds")]
        [SwaggerResponse(204, "Updates the accuracy of higher needs task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateAccuracyOfHigherNeedsTaskAsync(
            [FromBody] UpdateAccuracyOfHigherNeedsTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        } 

        /// <summary>
        /// Updating deed of termination for the master funding agreement for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/DeedOfTerminationMasterFundingAgreement")]
        [SwaggerResponse(204, "Updated deed of termination for the master funding agreement for transfer project task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateDeedOfTerminationMasterFundingAgreementTaskAsync(
            [FromBody] UpdateDeedOfTerminationMasterFundingAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }        

        /// <summary>
        /// Updating the chair of governors’ task data for conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/ConfirmChairOfGovernors")]
        [SwaggerResponse(204, "Updated the chair of governors’ task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateChairOfGovernorsTaskAsync(
            [FromBody] UpdateChairOfGovernorsCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updating the deed of termination for the church supplemental agreement task data for transfer project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/DeedTerminationChurchSupplementalAgreement")]
        [SwaggerResponse(204, "Updated the deed of termination for the church supplemental church agreement task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateDeedTerminationChurchSupplementalAgreementTaskAsync(
            [FromBody] UpdateDeedTerminationChurchSupplementalAgreementTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Request a new URN and record for the academy task for the project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/RequestNewURNAndRecordForAcademy")]
        [SwaggerResponse(204, "Request a new URN and record for the academy task updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateRequestNewUrnAndRecordForAcademyTaskAsync(
            [FromBody] UpdateRequestNewUrnAndRecordForAcademyTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
        
        /// <summary>
        /// Updating the 125 year lease for conversion project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("TaskData/OneHundredAndTwentyFiveYearLease")]
        [SwaggerResponse(204, "Updated the 125 year lease task successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateUpdateOneHundredAndTwentyFiveYearLeaseTaskAsync(
            [FromBody] UpdateOneHundredAndTwentyFiveYearLeaseTaskCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
    }
}