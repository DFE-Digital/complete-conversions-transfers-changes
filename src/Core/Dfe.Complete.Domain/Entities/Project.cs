using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dfe.Complete.Application.Tests")]
namespace Dfe.Complete.Domain.Entities;

public class Project : BaseAggregateRoot, IEntity<ProjectId>
{
    public ProjectId Id { get; set; }

    public Urn Urn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Ukprn? IncomingTrustUkprn { get; set; }

    public UserId RegionalDeliveryOfficerId { get; set; }

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

    public bool FormAMat => NewTrustReferenceNumber != null && NewTrustName != null;

    public GiasEstablishment? GiasEstablishment { get; internal set; }

    public virtual User? AssignedTo { get; set; }

    public virtual User? Caseworker { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual User? RegionalDeliveryOfficer { get; set; }

    public virtual LocalAuthority? LocalAuthority { get; set; }

    public virtual ICollection<SignificantDateHistory> SignificantDateHistories { get; set; } = new List<SignificantDateHistory>();


    internal Project()
    {
        Id = default!;
        Urn = default!;
        RegionalDeliveryOfficerId = default!;
        RegionalDeliveryOfficer = default!;
        LocalAuthorityId = default!;
        LocalAuthority = default!;
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
        string? advisoryBoardConditions,
        string? establishmentSharepointLink,
        string? incomingTrustSharepointLink,
        string? outgoingTrustSharepointLink,
        ProjectGroupId? groupId,
        ProjectTeam? team,
        UserId regionalDeliveryOfficerId,
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
    public static Project CreateConversionProject(CreateConversionProjectParams parameters)
    {
        var now = DateTime.UtcNow;

        var project = new Project(
            parameters.Id,
            parameters.Urn,
            now,
            now,
            TaskType.Conversion,
            ProjectType.Conversion,
            parameters.TasksDataId,
            parameters.SignificantDate,
            true,
            parameters.IncomingTrustUkprn,
            null,
            parameters.Region,
            false,
            parameters.HasAcademyOrderBeenIssued,
            parameters.AdvisoryBoardDate,
            parameters.AdvisoryBoardConditions,
            null,
            null,
            null,
            parameters.GroupId,
            null,
            parameters.RegionalDeliveryOfficerId,
            null,
            null,
            null,
            null,
            parameters.LocalAuthorityId)
        {
            State = ProjectState.Inactive
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

    public static Project CreateConversionMATProject(CreateConversionMatProjectParams parameters)
    {
        var now = DateTime.UtcNow;

        var project = new Project(
            parameters.Id,
            parameters.Urn,
            now,
            now,
            TaskType.Conversion,
            ProjectType.Conversion,
            parameters.TasksDataId,
            parameters.SignificantDate,
            true,
            null,
            null,
            parameters.Region,
            false,
            parameters.HasAcademyOrderBeenIssued,
            parameters.AdvisoryBoardDate,
            parameters.AdvisoryBoardConditions,
            null,
            null,
            null,
            null,
            null,
            parameters.RegionalDeliveryOfficerId,
            null,
            null,
            parameters.NewTrustName,
            parameters.NewTrustReferenceNumber,
            parameters.LocalAuthorityId)
        {
            State = ProjectState.Inactive
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

    public static Project CreateTransferProject(CreateTransferProjectParams parameters)
    {
        var now = DateTime.UtcNow;

        var project = new Project(
            parameters.Id,
            parameters.Urn,
            now,
            now,
            TaskType.Transfer,
            ProjectType.Transfer,
            parameters.TasksDataId,
            parameters.SignificantDate,
            true,
            parameters.IncomingTrustUkprn,
            parameters.OutgoingTrustUkprn,
            parameters.Region,
            false,
            null,
            parameters.AdvisoryBoardDate,
            parameters.AdvisoryBoardConditions,
            null,
            null,
            null,
            parameters.GroupId,
            null,
            parameters.RegionalDeliveryOfficerId,
            null,
            null,
            null,
            null,
            parameters.LocalAuthorityId)
        {
            State = ProjectState.Inactive,
            TwoRequiresImprovement = null
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

    public static Project CreateTransferMATProject(CreateTransferMatProjectParams parameters)
    {
        var now = DateTime.UtcNow;

        var project = new Project(
            parameters.Id,
            parameters.Urn,
            now,
            now,
            TaskType.Transfer,
            ProjectType.Transfer,
            parameters.TasksDataId,
            parameters.SignificantDate,
            true,
            null,
            parameters.OutgoingTrustUkprn,
            parameters.Region,
            false,
            null,
            parameters.AdvisoryBoardDate,
            parameters.AdvisoryBoardConditions,
            null,
            null,
            null,
            null,
            null,
            parameters.RegionalDeliveryOfficerId,
            null,
            null,
            parameters.NewTrustName,
            parameters.NewTrustReferenceNumber,
            parameters.LocalAuthorityId)
        {
            State = ProjectState.Inactive,
            TwoRequiresImprovement = null
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project));

        return project;
    }

    public void AddNote(Note? note)
    {
        if (note != null)
        {
            //Generate new note to ensure new Id
            Notes.Add(new Note
            {
                Id = new NoteId(Guid.NewGuid()),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                Body = note.Body,
                ProjectId = note.ProjectId,
                TaskIdentifier = note.TaskIdentifier,
                UserId = note.UserId,
                NotableId = note.NotableId,
                NotableType = note.NotableType
            });
        }
    }

    public void UpdateNote(Note updatedNote)
    {
        ArgumentNullException.ThrowIfNull(updatedNote);

        var note = Notes.FirstOrDefault(x => x.Id == updatedNote.Id) ?? throw new NotFoundException($"No note found with Id {updatedNote.Id.Value}");

        note.Body = updatedNote.Body;
        note.TaskIdentifier = updatedNote.TaskIdentifier;
        note.UserId = updatedNote.UserId;
        note.NotableId = updatedNote.NotableId;
        note.NotableType = updatedNote.NotableType;
    }

    public void RemoveNote(NoteId id)
    {
        var note = Notes.FirstOrDefault(x => x.Id == id) ?? throw new NotFoundException($"No note found with Id {id.Value}");
        Notes.Remove(note);
    }

    public void RemoveAllNotes()
    {
        // Create new id so we don't copy by reference as otherwise the list changes as we delete each note
        var noteIds = Notes.Select(note => note.Id).ToList();
        foreach (var noteId in noteIds)
        {
            RemoveNote(noteId);
        }
    }

    public void RemoveContact(ContactId id)
    {
        var contact = Contacts.FirstOrDefault(x => x.Id == id) ?? throw new NotFoundException($"No contact found with Id {id.Value}");
        Contacts.Remove(contact);
    }

    public void RemoveAllContacts()
    {
        // Create new id so we don't copy by reference as otherwise the list changes as we delete each contact
        var contactIds = Contacts.Select(contact => contact.Id).ToList();
        foreach (var contactId in contactIds)
        {
            RemoveContact(contactId);
        }
    }

    public void AddAcademyUrn(Urn urn)
    {
        AcademyUrn = urn;
    }

    public void UpdateSignificantDate(DateOnly date)
    {
        SignificantDate = date;
    }

    public void AddSignificantDateHistory(SignificantDateHistory significantDateHistory)
    {
        SignificantDateHistories.Add(new SignificantDateHistory
        {
            Id = significantDateHistory.Id,
            CreatedAt = significantDateHistory.CreatedAt,
            UpdatedAt = significantDateHistory.UpdatedAt,
            ProjectId = significantDateHistory.ProjectId,
            UserId = significantDateHistory.UserId,
            PreviousDate = significantDateHistory.PreviousDate,
            RevisedDate = significantDateHistory.RevisedDate,
        });
    }

    /// <summary>
    /// Raises a domain event when the project is assigned to the regional caseworker team.
    /// </summary>
    public void RaiseProjectAssignedToRegionalTeamEvent(string projectReference, string schoolName)
    {
        if (Type == null)
        {
            throw new InvalidOperationException("Cannot raise ProjectAssignedToRegionalTeamEvent: Project Type is null");
        }

        AddDomainEvent(new Events.ProjectAssignedToRegionalTeamEvent(
            Id,
            projectReference,
            Type.Value,
            Urn.Value,
            schoolName));
    }

    /// <summary>
    /// Raises a domain event when the project is assigned to a specific user.
    /// </summary>
    public void RaiseProjectAssignedToUserEvent(
        string projectReference,
        UserId assignedToUserId,
        string assignedToEmail,
        string assignedToFirstName,
        string schoolName)
    {
        if (Type == null)
        {
            throw new InvalidOperationException("Cannot raise ProjectAssignedToUserEvent: Project Type is null");
        }

        AddDomainEvent(new Events.ProjectAssignedToUserEvent(
            Id,
            projectReference,
            Type.Value,
            assignedToUserId,
            assignedToEmail,
            assignedToFirstName,
            Urn.Value,
            schoolName));
    }
}