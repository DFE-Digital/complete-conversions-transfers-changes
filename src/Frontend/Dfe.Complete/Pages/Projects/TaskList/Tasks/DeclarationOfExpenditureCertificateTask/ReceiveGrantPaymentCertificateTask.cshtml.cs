using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeclarationOfExpenditureCertificateTask
{
    public class ReceiveGrantPaymentCertificateTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ReceiveGrantPaymentCertificateTaskModel> logger, IErrorService errorService)
    : BaseDeclarationOfExpenditureCertificateTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ReceiveGrantPaymentCertificate, errorService)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.ReceiveGrantPaymentCertificateNotApplicable;
            CheckCertificate = ConversionTaskData.ReceiveGrantPaymentCertificateCheckCertificate;
            ReceivedDate = ConversionTaskData.ReceiveGrantPaymentCertificateDateReceived;
            Saved = ConversionTaskData.ReceiveGrantPaymentCertificateSaveCertificate;

            return Page();
        } 
    }
}
