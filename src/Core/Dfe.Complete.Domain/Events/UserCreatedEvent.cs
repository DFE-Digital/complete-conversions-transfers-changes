using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new user account is created by service support.
    /// Triggers sending of welcome/account creation email.
    /// </summary>
    public record UserCreatedEvent(
        UserId UserId,
        string Email,
        string FirstName,
        string LastName,
        string Team,
        DateTime OccurredOn) : IDomainEvent
    {
        public UserCreatedEvent(UserId userId, string email, string firstName, string lastName, string team)
            : this(userId, email, firstName, lastName, team, DateTime.UtcNow)
        {
        }
    }
}

