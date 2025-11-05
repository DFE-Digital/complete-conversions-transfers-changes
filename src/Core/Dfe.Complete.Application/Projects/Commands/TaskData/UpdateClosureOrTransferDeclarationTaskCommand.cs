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
    public record UpdateClosureOrTransferDeclarationTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Saved,
        bool? Sent
    ) : IRequest<Result<bool>>;

    internal class UpdateClosureOrTransferDeclarationTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateClosureOrTransferDeclarationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateClosureOrTransferDeclarationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.ClosureOrTransferDeclarationNotApplicable = request.NotApplicable;
            tasksData.ClosureOrTransferDeclarationReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.ClosureOrTransferDeclarationCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.ClosureOrTransferDeclarationSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.ClosureOrTransferDeclarationSent = request.NotApplicable == true ? null : request.Sent;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
