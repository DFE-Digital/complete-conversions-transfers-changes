using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsForUserQueryResultModel(
    ProjectId ProjectId,
    Urn Urn,
    string SchoolOrAcademyName,
    ProjectType? ProjectType,
    bool IsFormAMat,
    string? IncomingTrustName,
    string? OutgoingTrustName,
    string LocalAuthority,
    DateOnly? ConversionOrTransferDate
);