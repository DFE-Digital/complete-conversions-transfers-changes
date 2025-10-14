using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.RequestNewUrnAndRecordForAcademy
{
    public class RequestNewUrnAndRecordForAcademyModel(ISender sender, IAuthorizationService authorizationService, ILogger<RequestNewUrnAndRecordForAcademyModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.RequestNewUrnAndRecordForAcademy)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            return Page();
        }
    }
}
