using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.KeyContacts.Queries;

public record GetKeyContactsForProjectQuery(ProjectId ProjectId) : IRequest<Result<KeyContactDto>>;

public class GetKeyContactsForProjectQueryHandler(IKeyContactReadRepository keyContactsRepository, ILogger<GetKeyContactsForProjectQueryHandler> logger, IMapper mapper)
    : IRequestHandler<GetKeyContactsForProjectQuery, Result<KeyContactDto>>
{
    public async Task<Result<KeyContactDto>> Handle(GetKeyContactsForProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyContacts = await keyContactsRepository.KeyContacts.FirstOrDefaultAsync(contact => contact.ProjectId != null && contact.ProjectId == request.ProjectId,
                    cancellationToken);

            return Result<KeyContactDto>.Success(keyContacts == null ? new KeyContactDto() : mapper.Map<KeyContactDto>(keyContacts));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetKeyContactsForProjectQueryHandler), request);
            return Result<KeyContactDto>.Failure(ex.Message);
        }
    }
}