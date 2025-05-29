using MediatR;

namespace Dfe.Complete.Models
{
    public abstract class BaseProjectTabPageModel(ISender sender, string currentNavigation) : BaseProjectPageModel(sender)
    {
        public const string TaskList = "task-list";
        public const string AboutProject = "task-project";
        public const string Notes = "notes";
        public const string ExternalContacts = "external-contacts";
        public const string InternalContacts = "internal-contacts";
        public const string ConversionDateHistory = "conversion-date-history";

        public string CurrentNavigationItem { get; init; } = currentNavigation;
    }
}
