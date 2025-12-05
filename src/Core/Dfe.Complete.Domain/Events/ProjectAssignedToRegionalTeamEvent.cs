using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Events
{
    /// <summary>
    /// Domain event raised when a project is assigned to the regional caseworker team.
    /// Triggers sending of notification emails to all active team leaders.
    /// </summary>
    public record ProjectAssignedToRegionalTeamEvent(
        ProjectId ProjectId,
        string ProjectReference,
        ProjectType ProjectType,
        int Urn,
        string SchoolName,
        DateTime OccurredOn) : IDomainEvent
    {
        public ProjectAssignedToRegionalTeamEvent(
            ProjectId projectId,
            string projectReference,
            ProjectType projectType,
            int urn,
            string schoolName)
            : this(projectId, projectReference, projectType, urn, schoolName, DateTime.UtcNow)
        {
        }
    }
}

