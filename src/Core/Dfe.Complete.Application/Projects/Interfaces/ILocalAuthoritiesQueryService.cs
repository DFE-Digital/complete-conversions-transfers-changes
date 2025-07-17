using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface ILocalAuthoritiesQueryService
    {
        IQueryable<LocalAuthorityQueryModel> ListAllLocalAuthorities(LocalAuthorityId? Id = null);
        Task<LocalAuthorityDetailsModel?> GetLocalAuthorityDetailsAsync(LocalAuthorityId? Id = null, CancellationToken cancellationToken = default);
    }
}
