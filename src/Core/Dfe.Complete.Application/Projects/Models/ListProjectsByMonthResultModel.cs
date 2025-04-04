using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Projects.Models;

public record ListProjectsByMonthResultModel(
    string? EstablishmentName,
    string Region,
    string LocalAuthority,
    string OutgoingTrust,
    ProjectId ProjectId,
    Urn Urn,
    string IncomingTrust,
    string AllConditionsMet,
    string ConfirmedAndOriginalDate,
    ProjectType? ProjectType);