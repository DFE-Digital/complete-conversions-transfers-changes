using Asp.Versioning;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dfe.Complete.Api.Controllers
{
    /// <summary>
    /// Test endpoints for email notifications.
    /// </summary>
    /// <remarks>
    /// These endpoints are for testing email functionality during development.
    /// They should be removed or secured before production deployment.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "CanReadWrite")]
    [Route("v{version:apiVersion}/[controller]")]
    public class TestEmailController(IEmailSender emailSender, ILogger<TestEmailController> logger) : ControllerBase
    {
        /// <summary>
        /// Send a test welcome email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="firstName">The recipient's first name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the email send operation.</returns>
        [HttpGet("welcome")]
        [SwaggerResponse(200, "Email sent successfully.", typeof(EmailTestResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Failed to send email.")]
        public async Task<IActionResult> SendWelcomeEmailAsync(
            [FromQuery] string email,
            [FromQuery] string firstName,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Test request: Send welcome email to {Email}", email);

            var emailAddress = EmailAddress.Create(email);
            var message = new EmailMessage(
                To: emailAddress,
                TemplateKey: "NewAccountAdded",
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", firstName }
                },
                Reference: $"test-welcome-{Guid.NewGuid().ToString()[..8]}"
            );

            var result = await emailSender.SendAsync(message, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new EmailTestResponse
                {
                    Success = true,
                    MessageId = result.Value?.ProviderMessageId,
                    Reference = result.Value?.Reference,
                    SentAt = result.Value?.SentAt,
                    Message = $"Email sent successfully to {email}"
                });
            }

            return StatusCode(500, new EmailTestResponse
            {
                Success = false,
                Error = result.Error,
                ErrorType = result.ErrorType.ToString(),
                Message = $"Failed to send email to {email}"
            });
        }

        /// <summary>
        /// Send a test project assignment email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="firstName">The recipient's first name.</param>
        /// <param name="projectRef">The project reference (default: TEST-PROJECT-001).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the email send operation.</returns>
        [HttpGet("assignment")]
        [SwaggerResponse(200, "Email sent successfully.", typeof(EmailTestResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Failed to send email.")]
        public async Task<IActionResult> SendAssignmentEmailAsync(
            [FromQuery] string email,
            [FromQuery] string firstName,
            [FromQuery] string projectRef = "TEST-PROJECT-001",
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Test request: Send assignment email to {Email} for project {ProjectRef}", email, projectRef);

            var emailAddress = EmailAddress.Create(email);
            var message = new EmailMessage(
                To: emailAddress,
                TemplateKey: "AssignedNotification",
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", firstName },
                    { "project_url", $"https://complete-conversions-transfers.education.gov.uk/projects/{projectRef}" }
                },
                Reference: $"test-assignment-{projectRef}-{Guid.NewGuid().ToString()[..8]}"
            );

            var result = await emailSender.SendAsync(message, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new EmailTestResponse
                {
                    Success = true,
                    MessageId = result.Value?.ProviderMessageId,
                    Reference = result.Value?.Reference,
                    SentAt = result.Value?.SentAt,
                    Message = $"Email sent successfully to {email}"
                });
            }

            return StatusCode(500, new EmailTestResponse
            {
                Success = false,
                Error = result.Error,
                ErrorType = result.ErrorType.ToString(),
                Message = $"Failed to send email to {email}"
            });
        }

        /// <summary>
        /// Send a test team leader conversion project email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="firstName">The recipient's first name.</param>
        /// <param name="projectRef">The project reference (default: TEST-CONV-001).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the email send operation.</returns>
        [HttpGet("team-leader-conversion")]
        [SwaggerResponse(200, "Email sent successfully.", typeof(EmailTestResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Failed to send email.")]
        public async Task<IActionResult> SendTeamLeaderConversionEmailAsync(
            [FromQuery] string email,
            [FromQuery] string firstName,
            [FromQuery] string projectRef = "TEST-CONV-001",
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Test request: Send team leader conversion email to {Email} for project {ProjectRef}", email, projectRef);

            var emailAddress = EmailAddress.Create(email);
            var message = new EmailMessage(
                To: emailAddress,
                TemplateKey: "NewConversionProjectCreated",
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", firstName },
                    { "project_url", $"https://complete-conversions-transfers.education.gov.uk/projects/{projectRef}" }
                },
                Reference: $"test-tl-conv-{projectRef}-{Guid.NewGuid().ToString()[..8]}"
            );

            var result = await emailSender.SendAsync(message, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new EmailTestResponse
                {
                    Success = true,
                    MessageId = result.Value?.ProviderMessageId,
                    Reference = result.Value?.Reference,
                    SentAt = result.Value?.SentAt,
                    Message = $"Email sent successfully to {email}"
                });
            }

            return StatusCode(500, new EmailTestResponse
            {
                Success = false,
                Error = result.Error,
                ErrorType = result.ErrorType.ToString(),
                Message = $"Failed to send email to {email}"
            });
        }

        /// <summary>
        /// Send a test team leader transfer project email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="firstName">The recipient's first name.</param>
        /// <param name="projectRef">The project reference (default: TEST-TRANS-001).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the email send operation.</returns>
        [HttpGet("team-leader-transfer")]
        [SwaggerResponse(200, "Email sent successfully.", typeof(EmailTestResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Failed to send email.")]
        public async Task<IActionResult> SendTeamLeaderTransferEmailAsync(
            [FromQuery] string email,
            [FromQuery] string firstName,
            [FromQuery] string projectRef = "TEST-TRANS-001",
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Test request: Send team leader transfer email to {Email} for project {ProjectRef}", email, projectRef);

            var emailAddress = EmailAddress.Create(email);
            var message = new EmailMessage(
                To: emailAddress,
                TemplateKey: "NewTransferProjectCreated",
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", firstName },
                    { "project_url", $"https://complete-conversions-transfers.education.gov.uk/projects/{projectRef}" }
                },
                Reference: $"test-tl-trans-{projectRef}-{Guid.NewGuid().ToString()[..8]}"
            );

            var result = await emailSender.SendAsync(message, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new EmailTestResponse
                {
                    Success = true,
                    MessageId = result.Value?.ProviderMessageId,
                    Reference = result.Value?.Reference,
                    SentAt = result.Value?.SentAt,
                    Message = $"Email sent successfully to {email}"
                });
            }

            return StatusCode(500, new EmailTestResponse
            {
                Success = false,
                Error = result.Error,
                ErrorType = result.ErrorType.ToString(),
                Message = $"Failed to send email to {email}"
            });
        }
    }

    /// <summary>
    /// Response model for email test endpoints.
    /// </summary>
    public class EmailTestResponse
    {
        /// <summary>
        /// Indicates whether the email was sent successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The provider's message ID.
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// The reference used for this email.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// The timestamp when the email was sent.
        /// </summary>
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// A descriptive message about the operation result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Error details if the operation failed.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// The type of error that occurred.
        /// </summary>
        public string? ErrorType { get; set; }
    }
}


