using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForProjectQuery(ProjectId ProjectId) : IRequest<Result<List<Contact>>>;


public class GetContactsForProject(ICompleteRepository<Contact> contactsRepository) : IRequestHandler<GetContactsForProjectQuery, Result<List<Contact>>>
{

    public async Task<Result<List<Contact>>> Handle(GetContactsForProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contactsCollection =
                await contactsRepository.FetchAsync(contact => contact.ProjectId != null && contact.ProjectId == request.ProjectId,
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