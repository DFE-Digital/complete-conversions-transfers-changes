using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Entities;

public class Project : BaseAggregateRoot, IEntity<ProjectId>
{
    public ProjectId Id { get; set; }

    public Urn Urn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Ukprn? IncomingTrustUkprn { get; set; }

    public UserId? RegionalDeliveryOfficerId { get; set; }

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

    public string? Region { get; set; }

    public Urn? AcademyUrn { get; set; }

    public Guid? TasksDataId { get; set; }

    public TaskType? TasksDataType { get; set; }

    public Ukprn? OutgoingTrustUkprn { get; set; }

    public string? Team { get; set; }

    public bool? TwoRequiresImprovement { get; set; }

    public string? OutgoingTrustSharepointLink { get; set; }

    public bool? AllConditionsMet { get; set; }

    public ContactId? MainContactId { get; set; }

    public ContactId? EstablishmentMainContactId { get; set; }

    public ContactId? IncomingTrustMainContactId { get; set; }

    public ContactId? OutgoingTrustMainContactId { get; set; }

    public string? NewTrustReferenceNumber { get; set; }

    public string? NewTrustName { get; set; }

    public int State { get; set; }

    public int? PrepareId { get; set; }

    public ContactId? LocalAuthorityMainContactId { get; set; }

    public Guid? GroupId { get; set; }

    public virtual User? AssignedTo { get; set; }

    public virtual User? Caseworker { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual User? RegionalDeliveryOfficer { get; set; }
    
    private Project()
    {
    }

    public Project(ProjectId id,
        Urn urn,
        DateTime createdAt,
        DateTime updatedAt,
        TaskType taskType,
        ProjectType projectType,
        Guid tasksDataId,
        DateOnly significantDate,
        bool isSignificantDateProvisional,
        Ukprn incomingTrustUkprn,
        Ukprn? outgoingTrustUkprn,
        string? region,
        bool isDueTo2RI,
        bool? hasAcademyOrderBeenIssued,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        string? outgoingTrustSharepointLink,
        Guid? groupId,
        string team,
        UserId? regionalDeliveryOfficerId, 
        UserId? assignedTo, 
        DateTime? assignedAt)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Urn = urn ?? throw new ArgumentNullException(nameof(urn));
        CreatedAt = createdAt != default ? createdAt : throw new ArgumentNullException(nameof(createdAt));
        UpdatedAt = updatedAt != default ? updatedAt : throw new ArgumentNullException(nameof(updatedAt));
        TasksDataType = taskType;
        Type = projectType; //TOD EA: Comeback and validate the rest
        TasksDataId = tasksDataId;
        SignificantDate = significantDate;
        SignificantDateProvisional = isSignificantDateProvisional;
        IncomingTrustUkprn = incomingTrustUkprn;
        OutgoingTrustUkprn = outgoingTrustUkprn;
        TwoRequiresImprovement = isDueTo2RI;
        DirectiveAcademyOrder = hasAcademyOrderBeenIssued;
        AdvisoryBoardDate = advisoryBoardDate;
        AdvisoryBoardConditions = advisoryBoardConditions;
        EstablishmentSharepointLink = establishmentSharepointLink;
        IncomingTrustSharepointLink = incomingTrustSharepointLink;
        OutgoingTrustSharepointLink = outgoingTrustSharepointLink;
        GroupId = groupId;
        Team = team;
        RegionalDeliveryOfficerId = regionalDeliveryOfficerId;
        Region = region;

        AssignedAt = assignedAt;
        AssignedToId = assignedTo;
    }


    public static Project CreateConversionProject(
        ProjectId Id,
        Urn urn,
        DateTime createdAt,
        DateTime updatedAt,
        TaskType taskType,
        ProjectType projectType,
        Guid tasksDataId,
        DateOnly significantDate,
        bool isSignificantDateProvisional,
        Ukprn incomingTrustUkprn,
        string? region,
        bool isDueTo2RI,
        bool? hasAcademyOrderBeenIssued,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        Guid? groupId,
        string team, 
        UserId? regionalDeliveryOfficerId,
        UserId? assignedToId, 
        DateTime? assignedAt)
    {
        var project = new Project(
            Id,
            urn,
            createdAt,
            updatedAt,
            taskType,
            projectType,
            tasksDataId,
            significantDate,
            isSignificantDateProvisional,
            incomingTrustUkprn,
            null,
            region,
            isDueTo2RI,
            hasAcademyOrderBeenIssued,
            advisoryBoardDate,
            advisoryBoardConditions,
            establishmentSharepointLink,
            incomingTrustSharepointLink,
            null,
            groupId,
            team,
            regionalDeliveryOfficerId, 
            assignedToId, 
            assignedAt);

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }


    public static Project CreateTransferProject
        (
            ProjectId Id,
            Urn urn,
            DateTime createdAt,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            string? region,
            string team,
            UserId? regionalDeliveryOfficerId,
            UserId? assignedToId,
            DateTime? assignedAt,
            Ukprn incomingTrustUkprn,
            Ukprn outgoingTrustUkprn,
            Guid? groupId,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink,
            string outgoingTrustSharepointLink,
            DateOnly advisoryBoardDate,
            string advisoryBoardConditions,
            DateOnly significantDate,
            bool isSignificantDateProvisional,
            bool isDueTo2RI
        )
    {
        var project = new Project(
            Id,
            urn, 
            createdAt, 
            updatedAt, 
            taskType, 
            projectType, 
            tasksDataId, 
            significantDate, 
            isSignificantDateProvisional, 
            incomingTrustUkprn, 
            outgoingTrustUkprn,
            region, 
            isDueTo2RI, 
            null, 
            advisoryBoardDate, 
            advisoryBoardConditions, 
            establishmentSharepointLink, 
            incomingTrustSharepointLink,
            outgoingTrustSharepointLink, 
            groupId,
            team, 
            regionalDeliveryOfficerId, 
            assignedToId, 
            assignedAt);

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }
}