using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Domain.Entities;

public record CreateHandoverConversionProjectParams(
    ProjectId Id,
    Urn Urn,
    Guid TasksDataId,
    DateOnly SignificantDate,
    Ukprn IncomingTrustUkprn,
    Region? Region,
    bool HasAcademyOrderBeenIssued,
    DateOnly AdvisoryBoardDate,
    string? AdvisoryBoardConditions,
    ProjectGroupId? GroupId,
    UserId RegionalDeliveryOfficerId,
    Guid LocalAuthorityId);

public record CreateHandoverConversionMatProjectParams(
    ProjectId Id,
    Urn Urn,
    Guid TasksDataId,
    DateOnly SignificantDate,
    Region? Region,
    bool HasAcademyOrderBeenIssued,
    DateOnly AdvisoryBoardDate,
    string? AdvisoryBoardConditions,
    UserId RegionalDeliveryOfficerId,
    Guid LocalAuthorityId,
    string NewTrustReferenceNumber,
    string NewTrustName);

public record CreateHandoverTransferProjectParams(
    ProjectId Id,
    Urn Urn,
    Guid TasksDataId,
    DateOnly SignificantDate,
    Ukprn IncomingTrustUkprn,
    Ukprn OutgoingTrustUkprn,
    Region? Region,
    DateOnly AdvisoryBoardDate,
    string? AdvisoryBoardConditions,
    ProjectGroupId? GroupId,
    UserId RegionalDeliveryOfficerId,
    Guid LocalAuthorityId);