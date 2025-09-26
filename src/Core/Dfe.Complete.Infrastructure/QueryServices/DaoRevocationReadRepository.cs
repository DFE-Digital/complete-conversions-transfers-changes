using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore; 

namespace Dfe.Complete.Infrastructure.QueryServices
{
    internal class DaoRevocationReadRepository(CompleteContext ctx) : IDaoRevocationReadRepository
    {
        public IQueryable<DaoRevocation> DaoRevocations =>
          ctx.DaoRevocations
              .AsNoTracking();
    }
}
