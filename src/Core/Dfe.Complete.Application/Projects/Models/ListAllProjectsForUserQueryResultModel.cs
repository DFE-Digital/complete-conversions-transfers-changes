using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsForUserQueryResultModel(
    string SchoolOrAcademy,
    Urn Urn,
    ProjectType ProjectType,
    bool IsFormAMat,
    string? IncomingTrustUrn,
    string? OutgoingTrustUrn,
    string LocalAuthority,
    DateOnly ConversionOrTransferDate
);