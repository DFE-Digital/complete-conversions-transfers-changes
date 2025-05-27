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
            return await (
                from la in context.LocalAuthorities
                where la.Id == Id
                join c in context.Contacts on la.Id equals c.LocalAuthorityId into contacts
                from contact in contacts.DefaultIfEmpty()
                select new LocalAuthorityDetailsModel
                {
                    LocalAuthority = LocalAuthorityDto.MapLAEntityToDto(la),
                    Contact = contact != null ? ContactDetailsModel.MapContactEntityToModel(contact) : null
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
