using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Users.Models
{
    public record ListAllUsersQueryModel(User User)
    {
    }

    public record UsersFilters(
        string? Email = null!,
        string Team = null!
    );


    public record OrderUserQueryBy(OrderUserByField Field = OrderUserByField.Team, OrderByDirection Direction = OrderByDirection.Ascending);
}
