using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.AboutTheProject
{
    public class AboutTheProjectModel(ISender sender, ILogger<AboutTheProjectModel> logger) : ProjectLayoutModel(sender, logger, AboutTheProjectNavigation)
    {
        public GiasEstablishmentDto? Academy { get; set; }
        public ProjectGroupDto? ProjectGroup { get; set; }
        public TransferTaskDataDto? TransferTaskData { get; set; }

        private async Task SetAcademyAsync()
        {
            if (Project.AcademyUrn != null)
            {
                var academyQuery = new GetGiasEstablishmentByUrnQuery(Project.AcademyUrn);
                var academyResult = await Sender.Send(academyQuery);

                if (!academyResult.IsSuccess || academyResult.Value == null)
                {
                    throw new NotFoundException($"Academy {Project.AcademyUrn.Value} does not exist.");
                }

                Academy = academyResult.Value;
            }
        }

        private async Task SetProjectGroupAsync()
        {
            if (Project.GroupId != null)
            {
                var projectGroupQuery = new GetProjectGroupByIdQuery(Project.GroupId);
                var projectGroup = await Sender.Send(projectGroupQuery);
                if (projectGroup.IsSuccess || projectGroup.Value != null)
                {
                    ProjectGroup = projectGroup.Value;
                }
            }
        }

        private async Task SetTransferTaskDataAsync()
        {
            if (Project.TasksDataId != null)
            {
                var transferTasksDataQuery = new GetTransferTasksDataByIdQuery(Project.TasksDataId);
                var transferTasksData = await Sender.Send(transferTasksDataQuery);
                if (transferTasksData.IsSuccess || transferTasksData.Value != null)
                {
                    TransferTaskData = transferTasksData.Value;
                }
            }
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            //await base.OnGetAsync();
            var baseResult = await base.OnGetAsync();
            if (baseResult is not PageResult) return baseResult;

            await SetAcademyAsync();

            await SetProjectGroupAsync();

            await SetTransferTaskDataAsync();

            return Page();
        }
    }
}
