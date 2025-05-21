using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.ProjectView;

[ExcludeFromCodeCoverage]
public abstract class ProjectViewLayoutModel(ISender sender) : BaseProjectPageModel(sender)
{

}