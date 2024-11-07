using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Domain.Entities;

public class Project : BaseAggregateRoot, IEntity<ProjectId>
{
    public ProjectId Id { get; set; }

    public int Urn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? IncomingTrustUkprn { get; set; }

    public Guid? RegionalDeliveryOfficerId { get; set; }

    public Guid? CaseworkerId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateOnly? AdvisoryBoardDate { get; set; }

    public string? AdvisoryBoardConditions { get; set; }

    public string? EstablishmentSharepointLink { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? IncomingTrustSharepointLink { get; set; }

    public string? Type { get; set; }

    public Guid? AssignedToId { get; set; }

    public DateOnly? SignificantDate { get; set; }

    public bool? SignificantDateProvisional { get; set; }

    public bool? DirectiveAcademyOrder { get; set; }

    public string? Region { get; set; }

    public int? AcademyUrn { get; set; }

    public Guid? TasksDataId { get; set; }

    public string? TasksDataType { get; set; }

    public int? OutgoingTrustUkprn { get; set; }

    public string? Team { get; set; }

    public bool? TwoRequiresImprovement { get; set; }

    public string? OutgoingTrustSharepointLink { get; set; }

    public bool? AllConditionsMet { get; set; }

    public Guid? MainContactId { get; set; }

    public Guid? EstablishmentMainContactId { get; set; }

    public Guid? IncomingTrustMainContactId { get; set; }

    public Guid? OutgoingTrustMainContactId { get; set; }

    public string? NewTrustReferenceNumber { get; set; }

    public string? NewTrustName { get; set; }

    public int State { get; set; }

    public int? PrepareId { get; set; }

    public Guid? LocalAuthorityMainContactId { get; set; }

    public Guid? GroupId { get; set; }

    public virtual User? AssignedTo { get; set; }

    public virtual User? Caseworker { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual User? RegionalDeliveryOfficer { get; set; }
}
