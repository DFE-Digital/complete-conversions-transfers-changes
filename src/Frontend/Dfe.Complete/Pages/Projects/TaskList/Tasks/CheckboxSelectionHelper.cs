namespace Dfe.Complete.Pages.Projects.TaskList.Tasks;

public static class CheckboxSelectionHelper
{
    public static bool IsSelected(IEnumerable<string>? selectedOptions, string option)
    {
        return selectedOptions?.Contains(option, StringComparer.OrdinalIgnoreCase) == true;
    }

    public static List<string> BuildSelectedOptions(params (string Option, bool? Selected)[] mappings)
    {
        List<string> selectedOptions = [];

        foreach ((string option, bool? selected) in mappings)
        {
            if (selected == true)
            {
                selectedOptions.Add(option);
            }
        }

        return selectedOptions;
    }
}
