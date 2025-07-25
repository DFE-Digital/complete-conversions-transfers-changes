using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.ProjectView;

[ExcludeFromCodeCoverage]
public abstract class ProjectLayoutModel(ISender sender, ILogger _logger, string currentNavigation) : BaseProjectPageModel(sender, _logger)
{
    public string CurrentNavigationItem { get; } = currentNavigation;

    public const string TaskListNavigation = "task-list";
    public const string AboutTheProjectNavigation = "about-the-project";
    public const string NotesNavigation = "notes";
    public const string ExternalContactsNavigation = "external-contacts";
    public const string InternalContactsNavigation = "internal-contacts";
    public const string ConversionDateHistoryNavigation = "conversion-date-history";
}