using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmNurseryArrangementTask
{
    public class ConfirmNurseryArrangementTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmNurseryArrangementTaskModel> logger, IProjectPermissionService projectPermissionService)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.NurseryArrangement, projectPermissionService)
    {
        public List<NurseryArrangementOption> NurseryArrangementOptions { get; } = EnumExtensions.ToList<NurseryArrangementOption>();

        [BindProperty(Name = "not_applicable")]
        public bool? NotApplicable { get; set; }
        
        [BindProperty(Name = "nursery_arrangement")]
        public NurseryArrangementOption? SelectedNurseryArrangement { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            TasksDataId = Project.TasksDataId?.Value;

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            SelectedNurseryArrangement = ConversionTaskData.NurseryArrangement;
            NotApplicable = ConversionTaskData.NurseryArrangement == NurseryArrangementOption.NotApplicable;
            
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (NotApplicable == true)
            {
                SelectedNurseryArrangement = NurseryArrangementOption.NotApplicable;
            }
            
            await Sender.Send(new UpdateNurseryArrangementTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault()), 
                SelectedNurseryArrangement
            ));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
