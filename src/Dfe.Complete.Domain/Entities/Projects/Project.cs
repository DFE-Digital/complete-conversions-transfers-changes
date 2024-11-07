using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Contacts;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.Notes;
using Dfe.Complete.Domain.Users;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities.Projects
{
    public class Project : BaseAggregateRoot, IEntity<ProjectId>
    {
        public ProjectId Id { get; set; }

        public Urn Urn { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public UserId? TeamLeaderId { get; set; }

        public Ukprn? IncomingTrustUkprn { get; set; }

        public UserId? RegionalDeliveryOfficerId { get; set; }

        public UserId? CaseworkerId { get; set; }

        public DateTime? AssignedAt { get; set; }

        public DateTime? AdvisoryBoardDate { get; set; }

        public string AdvisoryBoardConditions { get; set; }

        public string EstablishmentSharepointLink { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string IncomingTrustSharepointLink { get; set; }

        public ProjectType Type { get; set; }

        public UserId? AssignedToId { get; set; }

        public DateTime? SignificantDate { get; set; }

        public bool? SignificantDateProvisional { get; set; }

        public bool? DirectiveAcademyOrder { get; set; }

        public Region? Region { get; set; }

        public Urn? AcademyUrn { get; set; }

        public Guid? TasksDataId { get; set; }

        public TaskType TasksDataType { get; set; }

        public Guid? FundingAgreementContactId { get; set; }

        public Ukprn? OutgoingTrustUkprn { get; set; }

        public ProjectTeam? Team { get; set; }

        public bool? TwoRequiresImprovement { get; set; }

        public string OutgoingTrustSharepointLink { get; set; }

        public bool? AllConditionsMet { get; set; }

        public UserId? MainContactId { get; set; }

        public UserId? EstablishmentMainContactId { get; set; }

        public UserId? IncomingTrustMainContactId { get; set; }

        public UserId? OutgoingTrustMainContactId { get; set; }

        public string NewTrustReferenceNumber { get; set; }

        public string NewTrustName { get; set; }

        public UserId? ChairOfGovernorsContactId { get; set; }

        public ProjectState State { get; set; }

        public virtual User AssignedTo { get; set; }

        public virtual User Caseworker { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

        public virtual User RegionalDeliveryOfficer { get; set; }

        public virtual User TeamLeader { get; set; }


        private Project() { }

        public Project(
            Urn urn,
            DateTime createdAt,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateTime significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateTime advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink
            )
        {
            Urn = urn ?? throw new ArgumentNullException(nameof(urn));
            CreatedAt = createdAt != default ? createdAt : throw new ArgumentNullException(nameof(createdAt));
            UpdatedAt = updatedAt != default ? updatedAt : throw new ArgumentNullException(nameof(updatedAt));
            TasksDataType = taskType;
            Type = projectType; //TOD EA: Comeback and validate the rest
            TasksDataId = tasksDataId;
            SignificantDate = significantDate;
            SignificantDateProvisional = isSignificantDateProvisional;
            IncomingTrustUkprn = incomingTrustUkprn;
            Region = region;
            TwoRequiresImprovement = isDueTo2RI;
            DirectiveAcademyOrder = hasAcademyOrderBeenIssued;
            AdvisoryBoardDate = advisoryBoardDate;
            AdvisoryBoardConditions = advisoryBoardConditions;
            EstablishmentSharepointLink = establishmentSharepointLink;
            IncomingTrustSharepointLink = incomingTrustSharepointLink;
        }


        public static Project Create(Urn urn,
            DateTime createdAt,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateTime significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateTime advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink)
        {

            var project = new Project(urn,
                                     createdAt,
                                     updatedAt,
                                     taskType,
                                     projectType,
                                     tasksDataId,
                                     significantDate,
                                     isSignificantDateProvisional,
                                     incomingTrustUkprn,
                                     region,
                                     isDueTo2RI,
                                     hasAcademyOrderBeenIssued,
                                     advisoryBoardDate,
                                     advisoryBoardConditions,
                                     establishmentSharepointLink,
                                     incomingTrustSharepointLink);

            project.AddDomainEvent(new ProjectCreatedEvent(project));


            return project;
        }



    }

}