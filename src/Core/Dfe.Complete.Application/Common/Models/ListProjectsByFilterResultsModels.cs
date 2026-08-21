using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Common.Models;
public record ListProjectsByFilterResultsModel(
    ProjectId ProjectId,
    string? EstablishmentName,
    Urn Urn,
    string ProjectType,
    bool? IsFormAMat,
    string? AssignedToFullName,
    Region? Region,
    string? LocalAuthorityName,
    DateOnly? SignificantDate,
    ProjectState State
);