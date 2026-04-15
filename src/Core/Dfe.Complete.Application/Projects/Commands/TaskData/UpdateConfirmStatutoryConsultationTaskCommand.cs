using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmStatutoryConsultationTaskCommand(
        TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? NotApplicable,
        bool? StatutoryConsultationComplete

    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmStatutoryConsultationTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmStatutoryConsultationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmStatutoryConsultationTaskCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectType == ProjectType.Conversion)
            {
                await UpdateConversionTaskDataAsync(request.TaskDataId, request, cancellationToken);
            }

            return Result<bool>.Success(true);
        }
        
        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateConfirmStatutoryConsultationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.StatutoryConsultationComplete = request.NotApplicable == true ? null : request.StatutoryConsultationComplete;
            tasksData.StatutoryConsultationNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);
        }
    }
}