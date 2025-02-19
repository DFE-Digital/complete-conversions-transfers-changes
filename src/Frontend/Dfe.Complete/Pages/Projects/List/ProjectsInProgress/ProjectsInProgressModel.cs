namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress;

public class ProjectsInProgressModel(string currentSubNavigationItem) : AllProjectsModel(InProgressNavigation)
{
    public const string AllProjectSubNavigation = "all-projects";
    public const string ConversionsSubNavigation = "conversions";
    public const string TransfersSubNavigation = "transfers";
    public const string FormAMatSubNavigation = "form-a-mat";
    public string CurrentSubNavigationItem { get; set; } = currentSubNavigationItem;
}