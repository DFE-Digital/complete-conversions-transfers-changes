using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities.Projects;
using Dfe.Complete.Domain.Entities.Schools;

namespace Dfe.Complete.Domain.Events
{
    public class ProjectCreatedEvent(Project project) : IDomainEvent
    {
        public Project Project { get; } = project;

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
