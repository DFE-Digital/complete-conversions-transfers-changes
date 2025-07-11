using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.LocalAuthorities.Queries
{
    public record GetLocalAuthorityDetailsQuery(LocalAuthorityId LocalAuthorityId) : IRequest<Result<LocalAuthorityDetailsModel>>;

    public class GetLocalAuthorityDetailsQueryHandler(ILocalAuthoritiesQueryService localAuthoritiesQueryService, ILogger<GetLocalAuthorityDetailsQueryHandler> logger)
       : IRequestHandler<GetLocalAuthorityDetailsQuery, Result<LocalAuthorityDetailsModel>>
    {
        public async Task<Result<LocalAuthorityDetailsModel>> Handle(GetLocalAuthorityDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var localAuthorityDetails = await localAuthoritiesQueryService
                    .GetLocalAuthorityDetailsAsync(request.LocalAuthorityId, cancellationToken);

                return localAuthorityDetails == null
                    ? throw new NotFoundException($"No local authority detail found for Id: {request.LocalAuthorityId.Value}.", nameof(request.LocalAuthorityId))
                    : Result<LocalAuthorityDetailsModel>.Success(localAuthorityDetails);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetLocalAuthorityDetailsQueryHandler), request);
                return Result<LocalAuthorityDetailsModel>.Failure(e.Message);
            }
        }
    }
}
