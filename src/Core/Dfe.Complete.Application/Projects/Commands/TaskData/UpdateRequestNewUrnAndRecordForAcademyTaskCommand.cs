using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateRequestNewUrnAndRecordForAcademyTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Complete,
        bool? Receive,
        bool? Give
    ) : IRequest<Result<bool>>;

    internal class UpdateRequestNewUrnAndRecordForAcademyTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateRequestNewUrnAndRecordForAcademyTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateRequestNewUrnAndRecordForAcademyTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.RequestNewUrnAndRecordNotApplicable = request.NotApplicable;
            tasksData.RequestNewUrnAndRecordComplete = request.NotApplicable == true ? null : request.Complete;
            tasksData.RequestNewUrnAndRecordReceive = request.NotApplicable == true ? null : request.Receive;
            tasksData.RequestNewUrnAndRecordGive = request.NotApplicable == true ? null : request.Give;
            
            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
            
            return Result<bool>.Success(true);
        }
    }
}
