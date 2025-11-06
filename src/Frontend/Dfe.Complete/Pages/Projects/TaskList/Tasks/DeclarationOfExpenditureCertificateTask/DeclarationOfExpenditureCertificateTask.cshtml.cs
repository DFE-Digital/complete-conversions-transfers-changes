using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeclarationOfExpenditureCertificateTask
{
    public class DeclarationOfExpenditureCertificateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeclarationOfExpenditureCertificateTaskModel> logger, IErrorService errorService)
    : BaseDeclarationOfExpenditureCertificateTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeclarationOfExpenditureCertificate, errorService)
    { 

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = TransferTaskData.DeclarationOfExpenditureCertificateNotApplicable;
            CheckCertificate = TransferTaskData.DeclarationOfExpenditureCertificateCorrect;
            ReceivedDate = TransferTaskData.DeclarationOfExpenditureCertificateDateReceived;
            Saved = TransferTaskData.DeclarationOfExpenditureCertificateSaved;

            return Page();
        }
    }
}
