namespace Dfe.Complete.Pages.Projects.ServiceSupport.ConversionURNs;

public class ConversionsUrnModel(string currentSubNavigationItem)
{
    public const string WithoutAcademyURNSubNavigation = "without-academy-urn";
    public const string WithAcademyURNSubNavigation = "with-academy-urn";
    
    public string CurrentSubNavigationItem { get; set; } = currentSubNavigationItem;
}