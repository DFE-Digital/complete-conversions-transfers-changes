#nullable enable

using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Dfe.Complete.Models.ProjectList;

public class ProjectListFilters
{
    public const string FilterStatuses = nameof(FilterStatuses);
    public const string FilterProjectOwners = nameof(FilterProjectOwners);
    public const string FilterRegions = nameof(FilterRegions);
    public const string FilterLocalAuthorities = nameof(FilterLocalAuthorities);

    private IDictionary<string, object?> _store = null!;

    public List<SignificantChangeFilterOption> StatusOptions { get; set; } = [];

    public List<SignificantChangeFilterOption> ProjectOwnerOptions { get; set; } = [];

    public List<SignificantChangeFilterOption> RegionOptions { get; set; } = [];

    public List<SignificantChangeFilterOption> LocalAuthorityOptions { get; set; } = [];

    public string? Keyword { get; set; }

    [BindProperty] public string[] SelectedStatuses { get; set; } = [];

    [BindProperty] public string[] SelectedProjectOwners { get; set; } = [];

    [BindProperty] public string[] SelectedRegions { get; set; } = [];

    [BindProperty] public string[] SelectedLocalAuthorities { get; set; } = [];

    public ProjectState[] SelectedStatusEnums =>
        [.. SelectedStatuses
            .Select(TryParseAs<ProjectState>)
            .Where(status => status.HasValue)
            .Select(status => status!.Value)
            .Distinct()];

    public bool IsVisible =>
        !string.IsNullOrWhiteSpace(Keyword)
        || SelectedStatuses.Length > 0
        || SelectedProjectOwners.Length > 0
        || SelectedRegions.Length > 0
        || SelectedLocalAuthorities.Length > 0;

    public SignificantChangeProjectFilters ToFilters() => new(
        ProjectStatuses: SelectedStatusEnums,
        Keyword: string.IsNullOrWhiteSpace(Keyword) ? null : Keyword.Trim(),
        ProjectOwnerIds: SelectedProjectOwners
            .Select(TryParseGuid)
            .Where(id => id.HasValue)
            .Select(id => new UserId(id!.Value))
            .ToArray(),
        Regions: SelectedRegions
            .Select(TryParseAs<Region>)
            .Where(region => region.HasValue)
            .Select(region => region!.Value)
            .Distinct()
            .ToArray(),
        LocalAuthorityIds: SelectedLocalAuthorities
            .Select(TryParseGuid)
            .Where(id => id.HasValue)
            .Select(id => new LocalAuthorityId(id!.Value))
            .ToArray());

    public string ToQueryString()
    {
        List<string> parts = [];

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            parts.Add($"{nameof(Keyword)}={Uri.EscapeDataString(Keyword.Trim())}");
        }

        AddParts(parts, nameof(SelectedStatuses), SelectedStatusEnums.Select(status => status.ToString()));
        AddParts(parts, nameof(SelectedProjectOwners), SelectedProjectOwners);
        AddParts(parts, nameof(SelectedRegions), SelectedRegions);
        AddParts(parts, nameof(SelectedLocalAuthorities), SelectedLocalAuthorities);

        return string.Join("&", parts);
    }

    private static void AddParts(List<string> parts, string key, IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            parts.Add($"{key}={Uri.EscapeDataString(value)}");
        }
    }

    public ProjectListFilters PersistUsing(IDictionary<string, object?> store)
    {
        _store = store;

        SelectedStatuses = Get(FilterStatuses);
        SelectedProjectOwners = Get(FilterProjectOwners);
        SelectedRegions = Get(FilterRegions);
        SelectedLocalAuthorities = Get(FilterLocalAuthorities);

        return this;
    }

    private string[] Get(string key, bool persist = false)
    {
        if (!_store.ContainsKey(key)) return [];

        var value = _store[key] as string[];
        if (persist) Cache(key, value);

        return value ?? [];
    }

    private string[] Cache(string key, string[]? value)
    {
        if (value is null || value.Length == 0)
            _store.Remove(key);
        else
            _store[key] = value;

        return value ?? [];
    }

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        Dictionary<string, StringValues> query = new(requestQuery, StringComparer.OrdinalIgnoreCase);

        if (query.ContainsKey("clear"))
        {
            ClearFilters();

            return;
        }

        bool activeFilterChanges = query.Keys.Any(IsFilterKey);

        if (activeFilterChanges)
        {
            Keyword = GetKeyword();
            SelectedStatuses = Cache(FilterStatuses, GetFromQuery(nameof(SelectedStatuses)));
            SelectedProjectOwners = Cache(FilterProjectOwners, GetFromQuery(nameof(SelectedProjectOwners)));
            SelectedRegions = Cache(FilterRegions, GetFromQuery(nameof(SelectedRegions)));
            SelectedLocalAuthorities = Cache(FilterLocalAuthorities, GetFromQuery(nameof(SelectedLocalAuthorities)));
        }
        else
        {
            SelectedStatuses = Get(FilterStatuses, true);
            SelectedProjectOwners = Get(FilterProjectOwners, true);
            SelectedRegions = Get(FilterRegions, true);
            SelectedLocalAuthorities = Get(FilterLocalAuthorities, true);
        }

        bool IsFilterKey(string key) =>
            key.Equals(nameof(Keyword), StringComparison.OrdinalIgnoreCase)
            || key.Equals(nameof(SelectedStatuses), StringComparison.OrdinalIgnoreCase)
            || key.Equals(nameof(SelectedProjectOwners), StringComparison.OrdinalIgnoreCase)
            || key.Equals(nameof(SelectedRegions), StringComparison.OrdinalIgnoreCase)
            || key.Equals(nameof(SelectedLocalAuthorities), StringComparison.OrdinalIgnoreCase);

        string? GetKeyword()
        {
            if (!query.TryGetValue(nameof(Keyword), out StringValues values)) return null;

            string? keyword = values.FirstOrDefault()?.Trim();

            return string.IsNullOrWhiteSpace(keyword) ? null : keyword;
        }

        string[] GetFromQuery(string key)
        {
            if (!query.TryGetValue(key, out StringValues values)) return [];

            return [.. values
                .SelectMany(value => (value ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct(StringComparer.OrdinalIgnoreCase)];
        }
    }

    private void ClearFilters()
    {
        Keyword = null;
        SelectedStatuses = Cache(FilterStatuses, default);
        SelectedProjectOwners = Cache(FilterProjectOwners, default);
        SelectedRegions = Cache(FilterRegions, default);
        SelectedLocalAuthorities = Cache(FilterLocalAuthorities, default);
    }

    public static void ClearFiltersFrom(IDictionary<string, object?> store)
    {
        new ProjectListFilters().PersistUsing(store).ClearFilters();
    }

    private static T? TryParseAs<T>(string? input) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (int.TryParse(input, out int intValue) && Enum.IsDefined(typeof(T), intValue))
            return (T)Enum.ToObject(typeof(T), intValue);

        if (Enum.TryParse(input, true, out T enumValue) && Enum.IsDefined(typeof(T), enumValue))
            return enumValue;

        return null;
    }

    private static Guid? TryParseGuid(string? input) =>
        Guid.TryParse(input, out Guid value) ? value : null;
}
