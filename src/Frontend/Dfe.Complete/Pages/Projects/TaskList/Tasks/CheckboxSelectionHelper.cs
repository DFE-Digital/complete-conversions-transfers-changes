namespace Dfe.Complete.Pages.Projects.TaskList.Tasks;

public static class CheckboxSelectionHelper
{
    public static bool IsSelected(IEnumerable<string>? selectedOptions, string option) => 
        selectedOptions?.Contains(option, StringComparer.OrdinalIgnoreCase) == true;

    public static List<string> BuildSelectedOptions(params (string Option, bool? Selected)[] mappings) => [.. mappings
        .Where(mapping => mapping.Selected == true)
        .Select(mapping => mapping.Option)];
}