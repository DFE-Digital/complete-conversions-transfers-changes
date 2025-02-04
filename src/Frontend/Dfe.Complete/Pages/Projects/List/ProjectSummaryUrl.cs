using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Pages.Projects.List;

public static class ProjectSummaryUrl
{
    public static string GetUrl(ListAllProjectsResultModel project)
    {
        return string.Format(project.ProjectType == ProjectType.Conversion ? RouteConstants.ConversionProjectTaskList : RouteConstants.TransferProjectTaskList, project.ProjectId);
    }
}