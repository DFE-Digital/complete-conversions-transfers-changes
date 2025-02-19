using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Users.Models;

public record UserWithProjectsDto(
    UserId Id,
    string FullName,
    string? Email,
    ProjectTeam? Team,
    List<ListAllProjectsResultModel> ProjectsAssigned,
    int ConversionProjectsAssigned,
    int TransferProjectsAssigned);