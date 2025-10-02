using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmDateAcademyTransferredTaskCommand(
        TaskDataId TaskDataId,
        DateOnly? DateAcademyTransferred
    ) : IRequest<Result<bool>>;

    public class UpdateConfirmDateAcademyTransferredTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmDateAcademyTransferredTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmDateAcademyTransferredTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");


            tasksData.ConfirmDateAcademyTransferredDateTransferred = request.DateAcademyTransferred;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken); 
            return Result<bool>.Success(true);
        }
    }
}
