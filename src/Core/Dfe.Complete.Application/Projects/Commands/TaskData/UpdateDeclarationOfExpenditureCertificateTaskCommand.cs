using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateDeclarationOfExpenditureCertificateTaskCommand(TaskDataId TaskDataId,
         ProjectType? ProjectType,
         DateTime? DateReceived,
         bool? NotApplicable,
         bool? CheckCertificate,
         bool? Saved) : IRequest<Result<bool>>;

    public class UpdateDeclarationOfExpenditureCertificateTaskCommandHandler(
       ITaskDataReadRepository taskDataReadRepository,
       ITaskDataWriteRepository taskDataWriteRepository)
       : IRequestHandler<UpdateDeclarationOfExpenditureCertificateTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeclarationOfExpenditureCertificateTaskCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectType == ProjectType.Conversion)
            {
                await UpdateConversionTaskDataAsync(request.TaskDataId, request, cancellationToken);
            }
            else if (request.ProjectType == ProjectType.Transfer)
            {
                await UpdateTransferTaskDataAsync(request.TaskDataId, request, cancellationToken);
            }

            return Result<bool>.Success(true);
        }


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateDeclarationOfExpenditureCertificateTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.ReceiveGrantPaymentCertificateSaveCertificate = request.NotApplicable == true ? null : request.Saved;
            tasksData.ReceiveGrantPaymentCertificateCheckCertificate = request.NotApplicable == true ? null : request.CheckCertificate;
            tasksData.ReceiveGrantPaymentCertificateDateReceived = request.NotApplicable == true ? null : request.DateReceived.ToDateOnly();
            tasksData.ReceiveGrantPaymentCertificateNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateDeclarationOfExpenditureCertificateTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.DeclarationOfExpenditureCertificateCorrect = request.NotApplicable == true ? null : request.CheckCertificate;
            tasksData.DeclarationOfExpenditureCertificateDateReceived = request.NotApplicable == true ? null : request.DateReceived.ToDateOnly();
            tasksData.DeclarationOfExpenditureCertificateSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.DeclarationOfExpenditureCertificateNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, cancellationToken);
        } 
    }
}
