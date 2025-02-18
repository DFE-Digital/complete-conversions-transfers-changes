using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class User : BaseAggregateRoot, IEntity<UserId>
{
    public UserId Id { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool? ManageTeam { get; set; }

    public bool AddNewProject { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ActiveDirectoryUserId { get; set; }

    public bool? AssignToProject { get; set; }

    public bool? ManageUserAccounts { get; set; }

    public string? ActiveDirectoryUserGroupIds { get; set; }

    public string? Team { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public bool? ManageConversionUrns { get; set; }

    public bool? ManageLocalAuthorities { get; set; }

    public DateTime? LatestSession { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Project> ProjectAssignedTos { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCaseworkers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectRegionalDeliveryOfficers { get; set; } = new List<Project>();
}
