#nullable enable

using Dfe.Complete.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Dfe.Complete.Models.ProjectList;

public class ProjectListFilters
{
    public const string FilterStatuses = nameof(FilterStatuses);

    private IDictionary<string, object?> _store = null!;

    public List<string> AvailableStatuses { get; set; } = [];

    [BindProperty] public string[] SelectedStatuses { get; set; } = [];

    public ProjectState[] SelectedStatusEnums =>
        [.. SelectedStatuses
            .Select(TryParseAs<ProjectState>)
            .Where(status => status.HasValue)
            .Select(status => status!.Value)
            .Distinct()];

    public bool IsVisible => SelectedStatuses.Length > 0;

    public ProjectListFilters PersistUsing(IDictionary<string, object?> store)
    {
        _store = store;

        SelectedStatuses = Get(FilterStatuses);

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
            SelectedStatuses = [];

            return;
        }

        bool activeFilterChanges = query.ContainsKey(nameof(SelectedStatuses));

        if (activeFilterChanges)
        {
            SelectedStatuses = Cache(FilterStatuses, GetFromQuery(nameof(SelectedStatuses)));
        }
        else
        {
            SelectedStatuses = Get(FilterStatuses, true);
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
        Cache(FilterStatuses, default);
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
}
