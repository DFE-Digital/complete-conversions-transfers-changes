using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyAndTrustFinancialInfoTask
{
    public class AcademyAndTrustFinancialInfoTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AcademyAndTrustFinancialInfoTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAndConfirmAcademyAndTrustFinancialInformation)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public string? AcademySurplusOrDeficit { get; set; }
        [BindProperty]
        public string? TrustSurplusOrDeficit { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; } 
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync(); 
            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = TransferTaskData.CheckAndConfirmFinancialInformationNotApplicable;
            AcademySurplusOrDeficit = TransferTaskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit;
            TrustSurplusOrDeficit = TransferTaskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await Sender.Send(new UpdateAcademyAndTrustFinancialInformationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, AcademySurplusOrDeficit, TrustSurplusOrDeficit));
            return OnPostProcessResponse(result);
        }
    }
}
