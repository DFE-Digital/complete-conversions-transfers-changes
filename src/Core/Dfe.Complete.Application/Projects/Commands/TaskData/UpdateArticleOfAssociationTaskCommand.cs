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
    public record UpdateArticleOfAssociationTaskCommand(
        TaskDataId TaskDataId,
        ProjectType? ProjectType,
        bool? NotApplicable,
        bool? ArticlesOfAssociationCleared,
        bool? ArticlesOfAssociationReceived,
        bool? ArticlesOfAssociationSent,
        bool? ArticlesOfAssociationSigned,
        bool? ArticlesOfAssociationSaved
    ) : IRequest<Result<bool>>;

    public class UpdateArticleOfAssociationTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateArticleOfAssociationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateArticleOfAssociationTaskCommand request, CancellationToken cancellationToken)
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


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateArticleOfAssociationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.ArticlesOfAssociationCleared = request.NotApplicable == true ? null : request.ArticlesOfAssociationCleared;
            tasksData.ArticlesOfAssociationNotApplicable = request.NotApplicable;
            tasksData.ArticlesOfAssociationReceived = request.NotApplicable == true ? null : request.ArticlesOfAssociationReceived;
            tasksData.ArticlesOfAssociationSent = request.NotApplicable == true ? null : request.ArticlesOfAssociationSent;
            tasksData.ArticlesOfAssociationSigned = request.NotApplicable == true ? null : request.ArticlesOfAssociationSigned;
            tasksData.ArticlesOfAssociationSaved = request.NotApplicable == true ? null : request.ArticlesOfAssociationSaved;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateArticleOfAssociationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.ArticlesOfAssociationCleared = request.NotApplicable == true ? null : request.ArticlesOfAssociationCleared;
            tasksData.ArticlesOfAssociationNotApplicable = request.NotApplicable;
            tasksData.ArticlesOfAssociationReceived = request.NotApplicable == true ? null : request.ArticlesOfAssociationReceived;
            tasksData.ArticlesOfAssociationSent = request.NotApplicable == true ? null : request.ArticlesOfAssociationSent;
            tasksData.ArticlesOfAssociationSigned = request.NotApplicable == true ? null : request.ArticlesOfAssociationSigned;
            tasksData.ArticlesOfAssociationSaved = request.NotApplicable == true ? null : request.ArticlesOfAssociationSaved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, cancellationToken);
        }
    }
}
