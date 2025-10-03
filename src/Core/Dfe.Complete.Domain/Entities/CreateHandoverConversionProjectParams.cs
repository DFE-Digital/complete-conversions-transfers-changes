using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Domain.Entities;

public class CreateHandoverConversionProjectParams
{
    public ProjectId Id { get; set; }
    public Urn Urn { get; set; }
    public Guid TasksDataId { get; set; }
    public DateOnly SignificantDate { get; set; }
    public Ukprn IncomingTrustUkprn { get; set; }
    public Region? Region { get; set; }
    public bool HasAcademyOrderBeenIssued { get; set; }
    public DateOnly AdvisoryBoardDate { get; set; }
    public string? AdvisoryBoardConditions { get; set; }
    public ProjectGroupId? GroupId { get; set; }
    public UserId RegionalDeliveryOfficerId { get; set; }
    public Guid LocalAuthorityId { get; set; }

    public CreateHandoverConversionProjectParams(
        ProjectId id,
        Urn urn,
        Guid tasksDataId,
        DateOnly significantDate,
        Ukprn incomingTrustUkprn,
        Region? region,
        bool hasAcademyOrderBeenIssued,
        DateOnly advisoryBoardDate,
        string? advisoryBoardConditions,
        ProjectGroupId? groupId,
        UserId regionalDeliveryOfficerId,
        Guid localAuthorityId)
    {
        Id = id;
        Urn = urn;
        TasksDataId = tasksDataId;
        SignificantDate = significantDate;
        IncomingTrustUkprn = incomingTrustUkprn;
        Region = region;
        HasAcademyOrderBeenIssued = hasAcademyOrderBeenIssued;
        AdvisoryBoardDate = advisoryBoardDate;
        AdvisoryBoardConditions = advisoryBoardConditions;
        GroupId = groupId;
        RegionalDeliveryOfficerId = regionalDeliveryOfficerId;
        LocalAuthorityId = localAuthorityId;
    }
}