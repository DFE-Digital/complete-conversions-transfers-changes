using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactsForProjectAndLocalAuthorityQuery(
    ProjectId ProjectId,
    LocalAuthorityId? LocalAuthorityId
) : IRequest<Result<List<ContactDto>?>>;

internal class GetContactsForProjectAndLocalAuthority(IContactReadRepository contactsRepository, IMapper mapper) : IRequestHandler<GetContactsForProjectAndLocalAuthorityQuery, Result<List<ContactDto>?>>
{
    public async Task<Result<List<ContactDto>?>> Handle(GetContactsForProjectAndLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contactsCollection =
                await contactsRepository.Contacts.Where(contact => contact.ProjectId != null && contact.ProjectId == request.ProjectId
                    || contact.LocalAuthorityId != null && contact.LocalAuthorityId == request.LocalAuthorityId)
                    .OrderBy(Contact => Contact.Name)
                    .ToListAsync(cancellationToken);
            var contactDto = mapper.Map<List<ContactDto>?>(contactsCollection);
            return Result<List<ContactDto>?>.Success(contactDto);
        }
        catch (Exception e)
        {
            return Result<List<ContactDto>?>.Failure(e.Message);
        }
    }
}