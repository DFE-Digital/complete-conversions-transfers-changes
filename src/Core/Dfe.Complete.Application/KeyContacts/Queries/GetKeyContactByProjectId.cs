using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Models;
using Dfe.Complete.Application.KeyContacts.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetKeyContactByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<KeyContactDto>>;

public class GetKeyContactByProjectIdQueryHandler(IKeyContactReadRepository keyContactReadRepository,
    IMapper mapper,
    ILogger<GetKeyContactByProjectIdQueryHandler> logger
) : IRequestHandler<GetKeyContactByProjectIdQuery, Result<KeyContactDto>>
{

    public async Task<Result<KeyContactDto>> Handle(
        GetKeyContactByProjectIdQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var keycontact = new KeyContactByProjectIdQuery(request.ProjectId)
                .Apply(keyContactReadRepository.KeyContacts);                    

            var keyContactDto = await keycontact
               .ProjectTo<KeyContactDto>(mapper.ConfigurationProvider)
               .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"Keycontact with Project ID {request.ProjectId.Value} not found");
            return Result<KeyContactDto>.Success(keyContactDto);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetKeyContactByProjectIdQueryHandler), request);
            return Result<KeyContactDto>.Failure(ex.Message);
        }
    }
}
