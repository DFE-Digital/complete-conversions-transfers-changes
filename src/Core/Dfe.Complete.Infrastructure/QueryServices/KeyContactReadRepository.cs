using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    internal class KeyContactReadRepository(CompleteContext ctx) : IKeyContactReadRepository
    {
        public IQueryable<KeyContact> KeyContacts =>
            ctx.KeyContacts
                .AsNoTracking();
    }
}
