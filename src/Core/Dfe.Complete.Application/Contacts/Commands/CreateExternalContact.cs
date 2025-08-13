using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Commands;

public record CreateExternalContactRequest(string FullName, 
    string Role, 
    string Email, 
    string PhoneNumber, 
    ContactCategory Category,
    bool IsPrimaryContact, 
    ProjectId? ProjectId, 
    int? EstablishmentUrn, 
    string? OrganisationName,
    LocalAuthorityId? LocalAuthorityId,
    ContactType? Type) : IRequest<ContactId>;


public class CreateExternalContact(ICompleteRepository<Contact> ContactRepository, ISender sender) : IRequestHandler<CreateExternalContactRequest, ContactId>
{
    public async Task<ContactId> Handle(CreateExternalContactRequest request, CancellationToken cancellationToken)
    {
        

        var contact = new Contact()
        {
            Category = request.Category,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
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

        if (request.IsPrimaryContact)
        {
            await sender.Send(new UpdatePrimaryContactAtOrganisationCommand(contact.ProjectId, result));
        }

        return result.Id;
    }
}