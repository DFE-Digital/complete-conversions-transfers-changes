using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForProjectByCategoryQuery(ProjectId ProjectId, ContactCategory ContactCategory) : IRequest<Result<List<ContactDto>>>;


internal class GetContactsForProjectByCategoryHandler(IContactReadRepository contactReadRepository, IMapper mapper) : IRequestHandler<GetContactsForProjectByCategoryQuery, Result<List<ContactDto>>>
{

    public async Task<Result<List<ContactDto>>> Handle(GetContactsForProjectByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filteredContacts = contactReadRepository.Contacts.Where(Contacts => Contacts.ProjectId == request.ProjectId && Contacts.Category == request.ContactCategory);

            var contactsCollection = await filteredContacts.ToListAsync(cancellationToken);
            var contactsDtoCollection = mapper.Map<List<ContactDto>>(contactsCollection);

            return Result<List<ContactDto>>.Success(contactsDtoCollection);
        }
        catch (Exception e)
        {
            return Result<List<ContactDto>>.Failure(e.Message);
        }
    }
}

