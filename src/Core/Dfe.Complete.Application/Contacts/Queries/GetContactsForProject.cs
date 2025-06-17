using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForProjectQuery(ProjectId ProjectId) : IRequest<Result<List<Contact>>>;


public class GetContactsForProject(IRepository<Contact> contactsRepository) : IRequestHandler<GetContactsForProjectQuery, Result<List<Contact>>>
{

    public async Task<Result<List<Contact>>> Handle(GetContactsForProjectQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}