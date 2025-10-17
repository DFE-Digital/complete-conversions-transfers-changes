using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{   public record UpdateLandRegistryTitlePlansTaskCommand(
        TaskDataId TaskDataId,
        bool? Received,
        bool? Cleared, 
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateLandRegistryTitlePlansTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateLandRegistryTitlePlansTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateLandRegistryTitlePlansTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.LandRegistryReceived = request.Received;
            tasksData.LandRegistryCleared = request.Cleared; 
            tasksData.LandRegistrySaved = request.Saved;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
