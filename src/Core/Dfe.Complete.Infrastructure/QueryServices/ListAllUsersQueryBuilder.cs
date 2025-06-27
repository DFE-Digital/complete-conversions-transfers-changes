using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    public class UsersQueryBuilder(CompleteContext context) : IUsersQueryBuilder
    {
        private IQueryable<User> _users = context.Users.AsQueryable();

        public IUsersQueryBuilder ApplyUsersFilters(UsersFilters filters)
        {
            _users = FilterByTeam(_users, filters.Team);
            _users = FilterByEmail(_users, filters.Email);

            return this;
        }
        public IUsersQueryBuilder Where(Expression<Func<User, bool>> predicate)
        {
            _users = _users.Where(predicate);
            return this;
        }
        private static IQueryable<User> FilterByTeam(IQueryable<User> users, string? team)
        {
            if (team != null)
                users = users.Where(user => user.Team == team);
            return users;
        }
        private static IQueryable<User> FilterByEmail(IQueryable<User> users, string? email)
        {
            if (email != null)
                users = users.Where(user => user.Email == email);
            return users;
        }

        public IQueryable<User> GetUsers()
        {
            return _users;
        }
        public IQueryable<ListAllUsersQueryModel> GenerateQuery(OrderUserQueryBy? orderBy = null)
        {
            return _users
                .OrderUserBy(orderBy)
                .Select(user => new ListAllUsersQueryModel(user));

        }
    }
}
