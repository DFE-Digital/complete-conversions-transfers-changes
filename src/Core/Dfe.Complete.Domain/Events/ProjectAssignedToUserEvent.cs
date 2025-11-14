using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Events
{
    /// <summary>
    /// Domain event raised when a project is assigned to a specific caseworker.
    /// Triggers sending of assignment notification email to the assigned user.
    /// </summary>
    public record ProjectAssignedToUserEvent(
        ProjectId ProjectId,
        string ProjectReference,
        ProjectType ProjectType,
        UserId AssignedToUserId,
        string AssignedToEmail,
        string AssignedToFirstName,
        int Urn,
        string SchoolName,
        DateTime OccurredOn) : IDomainEvent
    {
        public ProjectAssignedToUserEvent(
            ProjectId projectId,
            string projectReference,
            ProjectType projectType,
            UserId assignedToUserId,
            string assignedToEmail,
            string assignedToFirstName,
            int urn,
            string schoolName)
            : this(projectId, projectReference, projectType, assignedToUserId, assignedToEmail, assignedToFirstName, urn, schoolName, DateTime.UtcNow)
        {
        }
    }
}

