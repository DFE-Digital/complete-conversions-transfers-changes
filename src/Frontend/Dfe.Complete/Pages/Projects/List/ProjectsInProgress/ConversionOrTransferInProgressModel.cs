using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress;

public class ConversionOrTransferInProgressModel(string currentSubNavigationItem, ProjectType projectType) : ProjectsInProgressModel(currentSubNavigationItem)
{
        public ProjectType ProjectType { get; } = projectType;
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
}