using Dfe.Complete.Domain.Enums; 
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmAcademyRiskProtectionArrangements
{
    public class TransferConfirmAcademyRiskProtectionArrangementsModel(ISender sender, IAuthorizationService authorizationService, ILogger<TransferConfirmAcademyRiskProtectionArrangementsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmRiskProtectionArrangementsPolicy)
    {
        [BindProperty]
        public bool? RpaPolicyConfirm { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            RpaPolicyConfirm = TransferTaskData.RpaPolicyConfirm;
            return Page();
        } 
    }
}