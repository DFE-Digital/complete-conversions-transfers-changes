using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    public class UserReadRepository(CompleteContext ctx) : IUserReadRepository
    {
        public IQueryable<User> Users 
            => ctx.Users
            .AsNoTracking();
    }
}
