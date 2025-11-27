using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.ProjectView;

[ExcludeFromCodeCoverage]
public abstract class ProjectLayoutModel(ISender sender, ILogger logger, IProjectPermissionService projectPermissionService, string currentNavigation) : BaseProjectPageModel(sender, logger, projectPermissionService)
{
    public string CurrentNavigationItem { get; } = currentNavigation;

    public bool CanEditSignificantDate => Project != null && Project.SignificantDateProvisional is false && UserHasEditAccess();

    [BindProperty(SupportsGet = true, Name = "projectCompletionValidation")]
    public bool ShowProjectCompletionValidationNotification { get; set; }

    public const string TaskListNavigation = "task-list";
    public const string AboutTheProjectNavigation = "about-the-project";
    public const string NotesNavigation = "notes";
    public const string ExternalContactsNavigation = "external-contacts";
    public const string InternalContactsNavigation = "internal-contacts";
    public const string ConversionDateHistoryNavigation = "conversion-date-history";
    public const string RecordDaoRevocationNavigation = "dao-revocation";
}