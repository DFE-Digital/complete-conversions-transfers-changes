using Asp.Versioning;
using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ServiceSupportController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Creates a new local authority.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("LocalAuthority")]
        [SwaggerResponse(201, "Local authority created successfully.", typeof(Guid))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateLocalAuthorityAsync([FromBody] CreateLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            var localAuthorityId = await sender.Send(request, cancellationToken);
            return Created("", localAuthorityId.Value!.Value);
        }

        /// <summary>
        /// Returns a list of local authorities
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("LocalAuthority/List/All")]
        [SwaggerResponse(200, "List of local authorities.", typeof(List<LocalAuthorityQueryModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllLocalAuthoritiesAsync([FromQuery] ListLocalAuthoritiesQuery request, CancellationToken cancellationToken)
        {
            var localAuthories = await sender.Send(request, cancellationToken);
            return Ok(localAuthories.Value);
        }

        /// <summary>
        /// Get the local authority details
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("LocalAuthority")]
        [SwaggerResponse(200, "Local authority details.", typeof(LocalAuthorityDetailsModel))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetLocalAuthorityDetailsAsync([FromQuery] GetLocalAuthorityDetailsQuery request, CancellationToken cancellationToken)
        {
            var localAuthority = await sender.Send(request, cancellationToken);
            return Ok(localAuthority.Value);
        }

        /// <summary>
        /// Update the local authority details.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWriteUpdate")]
        [HttpPatch]
        [Route("LocalAuthority")]
        [SwaggerResponse(204, "local authoirty details updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> UpdateLocalAuthorityDetailsAsync([FromBody] UpdateLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Removes the local authority.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete]
        [Authorize(Policy = "CanReadWriteUpdateDelete")]
        [Route("LocalAuthority")]
        [SwaggerResponse(204, "local authority deleted successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> RemoveLocalAuthorityAsync(DeleteLocalAuthorityCommand request, CancellationToken cancellationToken)
        {  
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
    }
}
