using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.LocalAuthorities.Models;
using MediatR;

namespace Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;

public record GetLocalAuthorityByCodeQuery(string Code) : IRequest<Result<LocalAuthorityDto?>>;

public class GetLocalAuthorityByCodeQueryHandler(ICompleteRepository<LocalAuthority> localAuthorityRepo)
    : IRequestHandler<GetLocalAuthorityByCodeQuery, Result<LocalAuthorityDto?>>
{
    public async Task<Result<LocalAuthorityDto?>> Handle(GetLocalAuthorityByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var localAuthority = await localAuthorityRepo.GetAsync(x => x.Code == request.Code);

            if (localAuthority == null) return Result<LocalAuthorityDto?>.Success(null);

            return Result<LocalAuthorityDto?>.Success(LocalAuthorityDto.MapLAEntityToDto(localAuthority));
        }
        catch (Exception e)
        {
            return Result<LocalAuthorityDto?>.Failure(e.Message);
        }
    }
}