using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForLocalAuthorityQuery(LocalAuthorityId LocalAuthority) : IRequest<Result<List<Contact>>>;

public class GetContactsForLocalAuthority(ICompleteRepository<Contact> contactsRepository)
    : IRequestHandler<GetContactsForLocalAuthorityQuery, Result<List<Contact>>>
{
    public async Task<Result<List<Contact>>> Handle(GetContactsForLocalAuthorityQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contactsCollection =
                await contactsRepository.FetchAsync(
                    contact => contact.LocalAuthorityId != null &&
                               contact.LocalAuthorityId == request.LocalAuthority,
                    cancellationToken);
            var contactsList = contactsCollection.ToList();
            return Result<List<Contact>>.Success(contactsList);
        }
        catch (Exception e)
        {
            return Result<List<Contact>>.Failure(e.Message);
        }
        
    }
}