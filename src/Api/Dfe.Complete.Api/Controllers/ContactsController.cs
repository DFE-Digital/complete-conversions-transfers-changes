using Asp.Versioning;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.Services.PersonsApi;
using Dfe.Complete.Domain.Entities;
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
}