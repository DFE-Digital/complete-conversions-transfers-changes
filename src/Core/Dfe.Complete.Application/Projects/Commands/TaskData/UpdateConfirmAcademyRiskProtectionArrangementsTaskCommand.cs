using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand(
        [Required] TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? RpaPolicyConfirm,
        RiskProtectionArrangementOption? RpaOption,
        string? RpaReason
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmAcademyRiskProtectionArrangementsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectType == null)
            {
                return Result<bool>.Failure("Project type is required.");
            }
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


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.RiskProtectionArrangementOption = request.RpaOption;
            tasksData.RiskProtectionArrangementReason = request.RpaReason; 

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.RpaPolicyConfirm = request.RpaPolicyConfirm;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }
    }
}
