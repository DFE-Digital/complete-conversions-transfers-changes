#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Dfe.Complete.Models.ProjectList;

public class ProjectListFilters
{
    public const string FilterStatuses = nameof(FilterStatuses);

    private IDictionary<string, object?> _store = null!;

    public List<string> AvailableStatuses { get; set; } = [];

    [BindProperty] public string[] SelectedStatuses { get; set; } = [];

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
            if (query.TryGetValue(key, out StringValues values) is false) return [];

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
}
