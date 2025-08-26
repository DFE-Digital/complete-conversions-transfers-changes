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
    public record UpdateDeedOfVariationTaskCommand(
        TaskDataId TaskDataId,
        ProjectType? ProjectType,
        bool? Received,
        bool? Cleared,
        bool? Sent,
        bool? Saved,
        bool? Signed,
        bool? SignedSecretaryState,
        bool? NotApplicable) : IRequest<Result<bool>>;
    public class UpdateDeedOfVariationTaskCommandHandler(
       ITaskDataReadRepository taskDataReadRepository,
       ITaskDataWriteRepository taskDataWriteRepository)
       : IRequestHandler<UpdateDeedOfVariationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeedOfVariationTaskCommand request, CancellationToken cancellationToken)
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


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateDeedOfVariationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.DeedOfVariationSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.DeedOfVariationCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.DeedOfVariationReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.DeedOfVariationSent = request.NotApplicable == true ? null : request.Sent;
            tasksData.DeedOfVariationSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.DeedOfVariationSignedSecretaryState = request.NotApplicable == true ? null : request.SignedSecretaryState;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateDeedOfVariationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.DeedOfVariationSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.DeedOfVariationCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.DeedOfVariationReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.DeedOfVariationSent = request.NotApplicable == true ? null : request.Sent;
            tasksData.DeedOfVariationSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.DeedOfVariationSignedSecretaryState = request.NotApplicable == true ? null : request.SignedSecretaryState;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, cancellationToken);
        }
    }
}
