using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Dfe.Complete.Application.Contacts.Commands;

public record CreateExternalContactCommand(string FullName, 
    string Role, 
    string Email, 
    string PhoneNumber, 
    ContactCategory Category,
    bool IsPrimaryContact, 
    ProjectId? ProjectId, 
    int? EstablishmentUrn, 
    string? OrganisationName,
    LocalAuthorityId? LocalAuthorityId,
    ContactType? Type) : IRequest<Result<ContactId>>;

public class CreateExternalContactCommandHandler(
    ICompleteRepository<Contact> ContactRepository,
    ILogger<CreateExternalContactCommandHandler> logger,
    ISender sender) 
    : IRequestHandler<CreateExternalContactCommand, Result<ContactId>>
{
    public async Task<Result<ContactId>> Handle(CreateExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = new Contact()
            {
                Id = new ContactId(Guid.NewGuid()),
                Category = request.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Email = request.Email,
                Phone = request.PhoneNumber,
                EstablishmentUrn = request.EstablishmentUrn,
                LocalAuthorityId = request.LocalAuthorityId,
                Name = request.FullName,
                Title = request.Role,
                OrganisationName = request.OrganisationName,
                ProjectId = request.ProjectId,
                Type = request.Type
            };
            var result = await ContactRepository.AddAsync(contact, cancellationToken);

            await sender.Send(new UpdatePrimaryContactAtOrganisationCommand(contact.ProjectId!, request.IsPrimaryContact, result), cancellationToken);

            return Result<ContactId>.Success(result.Id);            
        }
        catch (Exception ex)
        {   
            var message = ErrorMessagesConstants.CouldNotCreateExternalContact.Replace("{Id}", request.ProjectId?.Value.ToString());
            logger.LogError(ex, ErrorMessagesConstants.CouldNotCreateExternalContact, request.ProjectId);            
            return Result<ContactId>.Failure(message);
        }
    }
}