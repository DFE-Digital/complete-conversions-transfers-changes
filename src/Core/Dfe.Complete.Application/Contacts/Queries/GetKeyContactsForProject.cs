using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetKeyContactsForProjectQuery(ProjectId ProjectId) : IRequest<Result<KeyContactsDto>>;

public class GetKeyContactsForProjectQueryHandler(ICompleteRepository<KeyContact> keyContactsRepository, IMapper mapper) 
    : IRequestHandler<GetKeyContactsForProjectQuery, Result<KeyContactsDto>>
{
    public async Task<Result<KeyContactsDto>> Handle(GetKeyContactsForProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyContacts = await keyContactsRepository.FindAsync(contact => contact.ProjectId != null && contact.ProjectId == request.ProjectId,
                    cancellationToken);

            return Result<KeyContactsDto>.Success(keyContacts == null ? new KeyContactsDto() : mapper.Map<KeyContactsDto>(keyContacts));
        }
        catch (Exception e)
        {
            return Result<KeyContactsDto>.Failure(e.Message);
        }
    }
}