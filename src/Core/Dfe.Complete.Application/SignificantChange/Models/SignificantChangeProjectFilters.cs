using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.SignificantChange.Models;

public record SignificantChangeProjectFilters(
    ProjectState[]? ProjectStatuses = null,
    string? Keyword = null,
    UserId[]? ProjectOwnerIds = null,
    Region[]? Regions = null,
    LocalAuthorityId[]? LocalAuthorityIds = null);

public record SignificantChangeFilterOption(string Value, string Label);

public record SignificantChangeProjectFilterOptions(
    List<SignificantChangeFilterOption> Statuses,
    List<SignificantChangeFilterOption> ProjectOwners,
    List<SignificantChangeFilterOption> Regions,
    List<SignificantChangeFilterOption> LocalAuthorities);
