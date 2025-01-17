using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Model;

public record ListAllProjectsResultModel(
    string? EstablishmentName,
    ProjectId ProjectId,
    Urn Urn,
    DateOnly? ConversionOrTransferDate,
    ProjectState State,
    ProjectType? ProjectType,
    bool IsFormAMAT,
    User? AssignedTo);