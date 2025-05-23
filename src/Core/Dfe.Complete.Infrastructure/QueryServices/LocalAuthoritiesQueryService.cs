using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    public class LocalAuthoritiesQueryService(CompleteContext context) : ILocalAuthoritiesQueryService
    {
        public IQueryable<LocalAuthorityQueryModel> ListAllLocalAuthorities(LocalAuthorityId? Id = null)
        {
            var localAuthorities = context.LocalAuthorities.AsQueryable();

            if (Id != null && Id.Value != Guid.Empty)
            {
                localAuthorities = localAuthorities.Where(x => x.Id == Id);
            }
            return localAuthorities
                .OrderBy(x => x.Name)
                .Select(la => new LocalAuthorityQueryModel
                {
                    Id = la.Id,
                    Name = la.Name
                });
        }

        public async Task<LocalAuthorityDetailsModel?> GetLocalAuthorityDetailsAsync(LocalAuthorityId? Id = null, CancellationToken cancellationToken = default)
        {
            return await context.LocalAuthorities
                .Where(la => la.Id == Id)
                .GroupJoin(
                    context.Contacts,
                    la => la.Id,
                    c => c.LocalAuthorityId,
                    (localAuthority, contacts) => new { LocalAuthority = localAuthority, Contact = contacts.FirstOrDefault() }
                )
                .Select(result => new LocalAuthorityDetailsModel
                {
                    LocalAuthority = LocalAuthorityDto.MapLAEntityToDto(result.LocalAuthority),
                    Contact = result.Contact != null ? ContactDetailsModel.MapContactEntityToModel(result.Contact) : null
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
