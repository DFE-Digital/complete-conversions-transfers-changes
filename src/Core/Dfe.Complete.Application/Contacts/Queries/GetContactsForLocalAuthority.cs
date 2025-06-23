using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForLocalAuthorityQuery(LocalAuthorityId LocalAuthority) : IRequest<Result<List<Contact>>>;


public class GetContactsForLocalAuthority(IRepository<Contact> contactsRepository) : IRequestHandler<GetContactsForLocalAuthorityQuery, Result<List<Contact>>>
{

    public async Task<Result<List<Contact>>> Handle(GetContactsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var contactsCollection =
            await contactsRepository.FetchAsync(contact => contact.LocalAuthorityId != null && contact.LocalAuthorityId.Value == request.LocalAuthority.Value,
                cancellationToken);
        var contactsList = contactsCollection.ToList();
        return Result<List<Contact>>.Success(contactsList);
    }
}