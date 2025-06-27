using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using System.Linq.Expressions; 

namespace Dfe.Complete.Application.Users.Interfaces
{
    public interface IUsersQueryBuilder
    {
        IUsersQueryBuilder ApplyUsersFilters(UsersFilters filters);
        IUsersQueryBuilder Where(Expression<Func<User, bool>> predicate);
        IQueryable<User> GetUsers();
        IQueryable<ListAllUsersQueryModel> GenerateQuery(OrderUserQueryBy? orderBy = null);
    }
}
