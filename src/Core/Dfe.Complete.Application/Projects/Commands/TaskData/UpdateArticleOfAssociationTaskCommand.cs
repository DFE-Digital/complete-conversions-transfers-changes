using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateArticleOfAssociationTaskCommand(
        TaskDataId TaskDataId,
        [Required]
        [ProjectType]
        ProjectType? ProjectType,
        bool? NotApplicable,
        bool? Cleared,
        bool? Received,
        bool? Sent,
        bool? Signed,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateArticleOfAssociationTaskCommandHandler(
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
             
            tasksData.ArticlesOfAssociationCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.ArticlesOfAssociationNotApplicable = request.NotApplicable;
            tasksData.ArticlesOfAssociationReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.ArticlesOfAssociationSent = request.NotApplicable == true ? null : request.Sent;
            tasksData.ArticlesOfAssociationSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.ArticlesOfAssociationSaved = request.NotApplicable == true ? null : request.Saved;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateArticleOfAssociationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.ArticlesOfAssociationCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.ArticlesOfAssociationNotApplicable = request.NotApplicable;
            tasksData.ArticlesOfAssociationReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.ArticlesOfAssociationSent = request.NotApplicable == true ? null : request.Sent;
            tasksData.ArticlesOfAssociationSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.ArticlesOfAssociationSaved = request.NotApplicable == true ? null : request.Saved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }
    }
}
