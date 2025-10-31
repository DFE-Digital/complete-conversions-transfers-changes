using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateDirectionToTransferTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Signed,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateDirectionToTransferTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateDirectionToTransferTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDirectionToTransferTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");
             
            tasksData.DirectionToTransferNotApplicable = request.NotApplicable;

            tasksData.DirectionToTransferReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.DirectionToTransferCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.DirectionToTransferSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.DirectionToTransferSaved = request.NotApplicable == true ? null : request.Saved;
            
            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
      
    }
}
