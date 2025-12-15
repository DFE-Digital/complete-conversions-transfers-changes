using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models
{
    public record ProjectDto
    {
        public ProjectId Id { get; set; } = default!;

        public Urn Urn { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Ukprn? IncomingTrustUkprn { get; set; }

        public UserId RegionalDeliveryOfficerId { get; set; } = default!;

        public UserId? CaseworkerId { get; set; }

        public DateTime? AssignedAt { get; set; }

        public DateOnly? AdvisoryBoardDate { get; set; }

        public string? AdvisoryBoardConditions { get; set; }

        public string? EstablishmentSharepointLink { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? IncomingTrustSharepointLink { get; set; }

        public ProjectType? Type { get; set; }

        public UserId? AssignedToId { get; set; }

        public DateOnly? SignificantDate { get; set; }

        public bool? SignificantDateProvisional { get; set; }

        public bool? DirectiveAcademyOrder { get; set; }

        public Region? Region { get; set; }

        public Urn? AcademyUrn { get; set; }

        public TaskDataId? TasksDataId { get; set; }

        public TaskType? TasksDataType { get; set; }

        public Ukprn? OutgoingTrustUkprn { get; set; }

        public ProjectTeam? Team { get; set; }

        public bool? TwoRequiresImprovement { get; set; }

        public string? OutgoingTrustSharepointLink { get; set; }

        public bool? AllConditionsMet { get; set; }

        public ContactId? MainContactId { get; set; }

        public ContactId? EstablishmentMainContactId { get; set; }

        public ContactId? IncomingTrustMainContactId { get; set; }

        public ContactId? OutgoingTrustMainContactId { get; set; }

        public string? NewTrustReferenceNumber { get; set; }

        public string? NewTrustName { get; set; }

        public ProjectState State { get; set; }

        public int? PrepareId { get; set; }

        public ContactId? LocalAuthorityMainContactId { get; set; }

        public ProjectGroupId? GroupId { get; set; }

        public User? AssignedTo { get; set; }

        public User? Caseworker { get; set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public ICollection<Note> Notes { get; set; } = new List<Note>();

        public ICollection<SignificantDateHistoryDto> SignificantDateHistories { get; set; } = new List<SignificantDateHistoryDto>();

        public User RegionalDeliveryOfficer { get; set; } = default!;

        // Updated, if both NewTrustReferenceNumber and NewTrustName are set, we can consider the project to have formed a MAT, removed incoming trust ukprn as null condition as that is not relevant here
        public bool FormAMat => !string.IsNullOrWhiteSpace(NewTrustReferenceNumber) && !string.IsNullOrWhiteSpace(NewTrustName);

        public string? EstablishmentName { get; set; }

        public LocalAuthorityId? LocalAuthorityId { get; set; }

    }
}
