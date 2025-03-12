namespace Dfe.Complete.Pages.Projects.List.ProjectsByMonth;

public class ProjectsByMonthModel(string currentSubNavigationItem) : AllProjectsModel(ByMonthNavigation)
{
    public const string ConversionsSubNavigation = "conversions";
    public const string TransfersSubNavigation = "transfers";
    public string CurrentSubNavigationItem { get; set; } = currentSubNavigationItem;
}