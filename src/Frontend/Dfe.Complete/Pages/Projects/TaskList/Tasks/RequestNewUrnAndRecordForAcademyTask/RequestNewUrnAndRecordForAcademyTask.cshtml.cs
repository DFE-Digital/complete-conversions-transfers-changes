using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.RequestNewUrnAndRecordForAcademyTask
{
    public class RequestNewUrnAndRecordForAcademyTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<RequestNewUrnAndRecordForAcademyTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.RequestNewUrnAndRecordForAcademy)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
