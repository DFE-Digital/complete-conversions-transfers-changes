using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsConvertingQueryResultModel(
    ProjectId ProjectId,
    string? EstablishmentName,
    int? Urn,
    DateOnly? ConversionDate,
    string? AcademyName,
    int? AcademyUrn
);