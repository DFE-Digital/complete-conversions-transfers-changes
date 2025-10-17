using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore; 

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateAccuracyOfHigherNeedsTaskCommand(
        TaskDataId TaskDataId,
        bool? ConfirmNumber,
        bool? ConfirmPublishedNumber
    ) : IRequest<Result<bool>>;

    internal class UpdateAccuracyOfHigherNeedsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateAccuracyOfHigherNeedsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateAccuracyOfHigherNeedsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.CheckAccuracyOfHigherNeedsConfirmNumber = request.ConfirmNumber;
            tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber = request.ConfirmPublishedNumber;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        } 
    } 
}
