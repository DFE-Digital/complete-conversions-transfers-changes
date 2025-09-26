using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.ProjectView;

[ExcludeFromCodeCoverage]
public abstract class ProjectLayoutModel(ISender sender, ILogger logger, string currentNavigation) : BaseProjectPageModel(sender, logger)
{

    [BindProperty(SupportsGet = true, Name = "projectCompletionValidation")]
    public bool ShowProjectCompletionValidationNotification { get; set; }

    public string CurrentNavigationItem { get; } = currentNavigation;

    public const string TaskListNavigation = "task-list";
    public const string AboutTheProjectNavigation = "about-the-project";
    public const string NotesNavigation = "notes";
    public const string ExternalContactsNavigation = "external-contacts";
    public const string InternalContactsNavigation = "internal-contacts";
    public const string ConversionDateHistoryNavigation = "conversion-date-history";
}