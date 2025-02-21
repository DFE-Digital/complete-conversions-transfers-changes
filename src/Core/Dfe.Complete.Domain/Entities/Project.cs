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
    
    public LocalAuthorityId LocalAuthorityId { get; set; }
    
    public bool FormAMat => NewTrustReferenceNumber != null && NewTrustName != null && IncomingTrustUkprn == null;
    
    public virtual User? AssignedTo { get; set; }

    public virtual User? Caseworker { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual User? RegionalDeliveryOfficer { get; set; }
    
    public virtual LocalAuthority LocalAuthority { get; set; }   
   
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
        Ukprn? incomingTrustUkprn,
        Ukprn? outgoingTrustUkprn,
        Region? region,
        bool isDueTo2RI,
        bool? hasAcademyOrderBeenIssued,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        string? outgoingTrustSharepointLink,
        ProjectGroupId? groupId,
        ProjectTeam? team,
        UserId? regionalDeliveryOfficerId,
        UserId? assignedTo,
        DateTime? assignedAt,
        string? newTrustName,
        string? newTrustReferenceNumber,
        Guid localAuthorityId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Urn = urn ?? throw new ArgumentNullException(nameof(urn));
        CreatedAt = createdAt != default ? createdAt : throw new ArgumentNullException(nameof(createdAt));
        UpdatedAt = updatedAt != default ? updatedAt : throw new ArgumentNullException(nameof(updatedAt));
        TasksDataType = taskType;
        Type = projectType; //TOD EA: Comeback and validate the rest
        TasksDataId = new TaskDataId(tasksDataId);
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

        NewTrustName = newTrustName;
        NewTrustReferenceNumber = newTrustReferenceNumber;

        LocalAuthorityId = new LocalAuthorityId(localAuthorityId);
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
        Region? region,
        bool isDueTo2RI,
        bool hasAcademyOrderBeenIssued,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        ProjectGroupId? groupId,
        ProjectTeam? team,
        UserId? regionalDeliveryOfficerId,
        UserId? assignedToId,
        DateTime? assignedAt,
        string? handoverComments,
        Guid localAuthorityId)
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
            assignedAt,
            null,
            null,
            localAuthorityId);

        if (!string.IsNullOrEmpty(handoverComments))
        {
            project.AddNote(new Note
            {
                CreatedAt = project.CreatedAt, ProjectId = project.Id, Body = handoverComments,
                TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(), UserId = assignedToId
            });
        }

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
        Region? region,
        ProjectTeam team,
        UserId? regionalDeliveryOfficerId,
        UserId? assignedToId,
        DateTime? assignedAt,
        Ukprn incomingTrustUkprn,
        Ukprn outgoingTrustUkprn,
        ProjectGroupId? groupId,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        string outgoingTrustSharepointLink,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        DateOnly significantDate,
        bool isSignificantDateProvisional,
        bool isDueTo2RI,
        string? handoverComments,
        Guid localAuthorityId
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
            assignedAt,
            null,
            null, 
            localAuthorityId);

        if (!string.IsNullOrEmpty(handoverComments))
        {
            project.AddNote(new Note
            {
                CreatedAt = project.CreatedAt, ProjectId = project.Id, Body = handoverComments,
                TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(), UserId = assignedToId
            });
        }

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

    public static Project CreateMatConversionProject(
        ProjectId Id,
        Urn urn,
        DateTime createdAt,
        DateTime updatedAt,
        TaskType taskType,
        ProjectType projectType,
        Guid tasksDataId,
        Region? region,
        ProjectTeam team,
        UserId? regionalDeliveryOfficerId,
        UserId? assignedToId,
        DateTime? assignedAt,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        DateOnly significantDate,
        bool isSignificantDateProvisional,
        bool isDueTo2Ri,
        string newTrustName,
        string newTrustReferenceNumber,
        bool hasDirectiveAcademyOrderBeenIssue,
        string? handoverComments, 
        Guid localAuthorityId)
    {
        var project = new Project(Id, urn, createdAt, updatedAt, taskType, projectType, tasksDataId, significantDate,
            isSignificantDateProvisional, null, null, region, isDueTo2Ri, hasDirectiveAcademyOrderBeenIssue,
            advisoryBoardDate, advisoryBoardConditions, establishmentSharepointLink, incomingTrustSharepointLink, null,
            null, team, regionalDeliveryOfficerId, assignedToId, assignedAt, newTrustName, newTrustReferenceNumber, localAuthorityId);
        
        if (!string.IsNullOrEmpty(handoverComments))
        {
            project.AddNote(new Note
            {
                CreatedAt = project.CreatedAt, ProjectId = project.Id, Body = handoverComments,
                TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(), UserId = assignedToId
            });
        }
        
        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

        public static Project CreateMatTransferProject(
        ProjectId Id,
        Urn urn,
        DateTime createdAt,
        DateTime updatedAt,
        Ukprn outgoingTrustUkprn,
        TaskType taskType,
        ProjectType projectType,
        Guid tasksDataId,
        Region? region,
        ProjectTeam team,
        UserId? regionalDeliveryOfficerId,
        UserId? assignedToId,
        DateTime? assignedAt,
        string establishmentSharepointLink,
        string incomingTrustSharepointLink,
        string outgoingTrustSharepointLink,
        DateOnly advisoryBoardDate,
        string advisoryBoardConditions,
        DateOnly significantDate,
        bool isSignificantDateProvisional,
        bool isDueTo2Ri,
        string newTrustName,
        string newTrustReferenceNumber,
        string? handoverComments, 
        Guid localAuthorityId)
        {
            var project = new Project(Id, urn, createdAt, updatedAt, taskType, projectType, tasksDataId, significantDate,
                isSignificantDateProvisional, null, outgoingTrustUkprn, region, isDueTo2Ri, null,
                advisoryBoardDate, advisoryBoardConditions, establishmentSharepointLink, incomingTrustSharepointLink, outgoingTrustSharepointLink,
                null, team, regionalDeliveryOfficerId, assignedToId, assignedAt, newTrustName, newTrustReferenceNumber, localAuthorityId);

            if (!string.IsNullOrEmpty(handoverComments))
            {
                project.AddNote(new Note
                {
                    CreatedAt = project.CreatedAt,
                    ProjectId = project.Id,
                    Body = handoverComments,
                    TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(),
                    UserId = assignedToId
                });
            }

            project.AddDomainEvent(new ProjectCreatedEvent(project));

            return project;
        }

    private void AddNote(Note? note)
    {
        if (note != null)
        {
            Notes.Add(new Note
            {
                Id = new NoteId(Guid.NewGuid()),
                CreatedAt = note.CreatedAt,
                Body = note.Body,
                ProjectId = note.ProjectId,
                TaskIdentifier = note.TaskIdentifier,
                UserId = note.User?.Id
            });
        }
    }
}