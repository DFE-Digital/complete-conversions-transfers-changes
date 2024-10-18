using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities.Schools;

namespace Dfe.Complete.Domain.Events
{
    public class SchoolCreatedEvent(School school) : IDomainEvent
    {
        public School School { get; } = school;

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
