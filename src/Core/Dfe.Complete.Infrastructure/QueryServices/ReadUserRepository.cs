using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    public class ReadUserRepository(CompleteContext ctx) : IReadUserRepository
    {
        public IQueryable<User> Users 
            => ctx.Users
            .AsNoTracking();
    }
}
