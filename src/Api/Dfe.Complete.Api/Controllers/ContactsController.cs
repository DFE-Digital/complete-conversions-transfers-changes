using Asp.Versioning;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.Services.PersonsApi;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dfe.Complete.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "CanRead")]
[Route("v{version:apiVersion}/[controller]")]
public class ContactsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Returns a list of Contacts for a specific Project
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanRead")]
    [HttpGet]
    [Route("List/Project")]
    [SwaggerResponse(200, "Contact", typeof(List<Contact>))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> ListAllContactsForProjectAsync([FromQuery] GetContactsForProjectQuery request, CancellationToken cancellationToken)
    {
        var contacts = await sender.Send(request, cancellationToken);
        return Ok(contacts.Value);
    }

    /// <summary>
    /// Returns a list of Contacts for a specific Project
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanRead")]
    [HttpGet]
    [Route("List/La")]
    [SwaggerResponse(200, "Contact", typeof(List<Contact>))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> ListAllContactsForLocalAuthorityAsync([FromQuery] GetContactsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var contacts = await sender.Send(request, cancellationToken);
        return Ok(contacts.Value);
    }

    /// <summary>
    /// Returns a list of Contacts for a specific Project and local authority 
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanRead")]
    [HttpGet]
    [Route("List/ProjectAndLocalAuthority")]
    [SwaggerResponse(200, "Contact", typeof(List<Contact>))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> ListAllContactsForProjectAndLocalAuthorityAsync([FromQuery] GetContactsForProjectAndLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var contacts = await sender.Send(request, cancellationToken);
        return Ok(contacts.Value);
    }

    /// <summary>
    /// Returns a parliament mp contact by a constituency
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanRead")]
    [HttpGet]
    [Route("GetParliamentMPContact")]
    [SwaggerResponse(200, "Contact", typeof(ConstituencyMemberContactDto))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> GetParliamentMPContactByConstituencyAsync([FromQuery] GetContactByConstituency request, CancellationToken cancellationToken)
    {
        var contacts = await sender.Send(request, cancellationToken);
        return Ok(contacts.Value);
    }

    /// <summary>
    /// Gets a external contact
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanRead")]
    [HttpGet]
    [SwaggerResponse(200, "Contact", typeof(ContactDto))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> GetExternalContactAsync([FromQuery] GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await sender.Send(request, cancellationToken);

        if (contact?.Value is null)
            return NotFound();

        return Ok(contact.Value);
    }

    /// <summary>
    /// Creates a new external contact for a project.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanReadWrite")]
    [HttpPost]
    [Route("CreateExternalContact")]
    [SwaggerResponse(201, "Contact", typeof(ContactId))]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> CreateExternalContactAsync([FromBody] CreateExternalContactCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);
        return Created("", result);
    }

    /// <summary>
    /// Updates external contact.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanReadWriteUpdate")]
    [HttpPut]
    [SwaggerResponse(204, "External contact updated successfully.")]
    [SwaggerResponse(400, "Invalid request data.")]
    [SwaggerResponse(404, "External contact not found.")]
    public async Task<IActionResult> UpdateExternaContactAsync([FromBody] UpdateExternalContactCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            if (response.ErrorType == ErrorType.NotFound)
                return NotFound(response.Error);
            return StatusCode(500, response.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes external contact.
    /// </summary>
    /// <param name="contactId">contact Id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Authorize(Policy = "CanReadWriteUpdateDelete")]
    [HttpDelete]
    [Route("DeleteExternalContact")]
    [SwaggerResponse(204, "External contact deleted successfully.")]
    [SwaggerResponse(400, "Invalid request data.")]
    public async Task<IActionResult> DeleteExternalContactAsync(ContactId contactId, CancellationToken cancellationToken)
    {
        if (contactId == null)
        {
            return BadRequest("Contact Id is required.");
        }

        var request = new DeleteExternalContactCommand(contactId);


        var response = await sender.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            if (response.ErrorType == ErrorType.NotFound)
                return NotFound(response.Error);
            return StatusCode(500, response.Error);
        }

        return NoContent();
    }
}