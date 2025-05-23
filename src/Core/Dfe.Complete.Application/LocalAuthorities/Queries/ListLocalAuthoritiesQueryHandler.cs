using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.LocalAuthorities.Queries
{
    public record GetLocalAuthoritiesQuery(int PageNumber, int PageCount) : PaginatedRequest<PaginatedResult<List<LocalAuthorityQueryModel>>>;

    public class ListLocalAuthoritiesQueryHandler(ILocalAuthoritiesQueryService localAuthoritiesQueryService, ILogger<ListLocalAuthoritiesQueryHandler> logger)
        : IRequestHandler<GetLocalAuthoritiesQuery, PaginatedResult<List<LocalAuthorityQueryModel>>>
    {
        public async Task<PaginatedResult<List<LocalAuthorityQueryModel>>> Handle(GetLocalAuthoritiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var localAuthoritiesQuery = localAuthoritiesQueryService
                    .ListAllLocalAuthorities();
                var localAuthoritiesCount = await localAuthoritiesQuery.CountAsync(cancellationToken);

                var localAuthorities = await localAuthoritiesQuery
                    .Skip(request.PageNumber * request.PageCount)
                    .Take(request.PageCount)
                    .ToListAsync(cancellationToken);

                return PaginatedResult<List<LocalAuthorityQueryModel>>.Success(localAuthorities, localAuthoritiesCount);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListLocalAuthoritiesQueryHandler), request);
                return PaginatedResult<List<LocalAuthorityQueryModel>>.Failure(e.Message);
            }
        } 
    }
}
