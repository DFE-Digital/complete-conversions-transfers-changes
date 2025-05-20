using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;

public record GetLocalAuthorityBySchoolUrnQuery(int SchoolUrn) : IRequest<Result<GetLocalAuthorityBySchoolUrnResponseDto?>>;

public record GetLocalAuthorityBySchoolUrnResponseDto(Guid? LocalAuthorityId);

public class GetLocalAuthorityBySchoolUrnQueryHandler(ICompleteRepository<LocalAuthority> localAuthorityRepo, ICompleteRepository<GiasEstablishment> giasEstablishmentRepo, ILogger<GetLocalAuthorityBySchoolUrnQueryHandler> logger) 
    : IRequestHandler<GetLocalAuthorityBySchoolUrnQuery, Result<GetLocalAuthorityBySchoolUrnResponseDto?>>
{
    public async Task<Result<GetLocalAuthorityBySchoolUrnResponseDto?>> Handle(GetLocalAuthorityBySchoolUrnQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var giasEstablishment = await giasEstablishmentRepo.GetAsync(x => x.Urn == new Urn(request.SchoolUrn));
            
            var localAuthority = await localAuthorityRepo.GetAsync(x => x.Code == giasEstablishment.LocalAuthorityCode);
            
            return Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(localAuthority.Id.Value));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetLocalAuthorityBySchoolUrnQueryHandler), request);
            return Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Failure(e.Message);
        }
    }
}