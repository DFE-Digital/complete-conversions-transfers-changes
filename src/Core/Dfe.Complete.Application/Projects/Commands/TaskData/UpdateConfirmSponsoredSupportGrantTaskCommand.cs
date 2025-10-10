using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmSponsoredSupportGrantTaskCommand(
        [Required] TaskDataId TaskDataId,
        [Required] [ProjectType] ProjectType? ProjectType,
        bool? NotApplicable,
        string? SponsoredSupportGrantType,
        bool? PaymentAmount,
        bool? PaymentForm,
        bool? SendInformation,
        bool? InformTrust
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmSponsoredSupportGrantTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmSponsoredSupportGrantTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmSponsoredSupportGrantTaskCommand request, CancellationToken cancellationToken)
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

        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateConfirmSponsoredSupportGrantTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");
            
            tasksData.SponsoredSupportGrantNotApplicable = request.NotApplicable;
            tasksData.SponsoredSupportGrantType = request.NotApplicable == true ? null : request.SponsoredSupportGrantType;

            tasksData.SponsoredSupportGrantPaymentAmount = request.NotApplicable == true ? null : request.PaymentAmount;
            tasksData.SponsoredSupportGrantPaymentForm = request.NotApplicable == true ? null : request.PaymentForm;
            tasksData.SponsoredSupportGrantSendInformation = request.NotApplicable == true ? null : request.SendInformation;
            tasksData.SponsoredSupportGrantInformTrust = request.NotApplicable == true ? null : request.InformTrust;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateConfirmSponsoredSupportGrantTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.SponsoredSupportGrantNotApplicable = request.NotApplicable;
            tasksData.SponsoredSupportGrantType = request.NotApplicable == true ? null : request.SponsoredSupportGrantType;
            
            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }
    }
}
