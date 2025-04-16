namespace Dfe.Complete.Models;

public class TabNavigationModel(string currentTab)
{
    public const string ViewDataKey = "TabNavigationModel";
    public const string YourProjectsTabName = "your-projects";
    public const string YourTeamProjectsTabName = "your-team-projects";
    public const string AllProjectsTabName = "all-projects";
    public const string GroupsTabName = "groups";
    public const string ServiceSupportProjectsTabName = "service-support";

    public string CurrentTab { get; } = currentTab;
}