using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.SignificantChange.Models;
public record ListSignificantChangeProjectsByFilterResultsModel(
    ProjectId ProjectId,
    string? EstablishmentName,
    Urn Urn,
    string? AssignedToFullName,
    Region? Region,
    string? LocalAuthorityName,
    DateOnly? SignificantDate,
    ProjectState State
);