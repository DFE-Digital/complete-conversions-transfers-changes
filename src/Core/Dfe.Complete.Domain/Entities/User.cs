using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;

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

    public string? EntraUserObjectId { get; set; }

    [Obsolete("Use IsAssignableToProject property instead")]
    public bool? AssignToProject { get; set; }

    public bool? ManageUserAccounts { get; set; }

    public string? ActiveDirectoryUserGroupIds { get; set; }

    public string? Team { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public bool? ManageConversionUrns { get; set; }

    public bool? ManageLocalAuthorities { get; set; }

    public DateTime? LatestSession { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public bool IsActive => DeactivatedAt == null;

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Project> ProjectAssignedTos { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCaseworkers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectRegionalDeliveryOfficers { get; set; } = new List<Project>();

    public bool IsAssignableToProject
    {
        get
        {
            if (string.IsNullOrEmpty(Team))
                return false;

            try
            {
                var userTeam = EnumExtensions.FromDescription<ProjectTeam>(Team);
                return userTeam.TeamIsRegionalDeliveryOfficer() || userTeam.TeamIsRegionalCaseworkServices();
            }
            catch
            {
                return false;
            }
        }
    }

    public static User Create(
        UserId id,
        string email,
        string firstName,
        string lastName,
        string? team)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Team = team,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ActiveDirectoryUserId = null,
            EntraUserObjectId = null,
            ActiveDirectoryUserGroupIds = null,
            ManageUserAccounts = null,
            ManageConversionUrns = false,
            ManageLocalAuthorities = false,
            ManageTeam = false,
            AssignToProject = null,
            AddNewProject = false,
            DeactivatedAt = null,
            LatestSession = null
        };

        //event to trigger welcome email
        user.AddDomainEvent(new Events.UserCreatedEvent(id, email, firstName, lastName, team ?? string.Empty));

        return user;
    }
}
