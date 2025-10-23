using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmBankDetailsTaskCommand(
        TaskDataId TaskDataId,
        bool? BankDetailsChangingYesNo
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmBankDetailsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmBankDetailsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmBankDetailsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");
             
            tasksData.BankDetailsChangingYesNo = request.BankDetailsChangingYesNo; 

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
