using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Users.Models;

public record ListAllUsersWithProjectsResultModel(
    UserId Id,
    string FullName,
    string? Email,
    ProjectTeam? Team,
    int ConversionProjectsAssigned,
    int TransferProjectsAssigned,
    DateTime? LatestSession = null);