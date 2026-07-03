using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateTenancyAtWillTaskCommand(
        TaskDataId TaskDataId,
        bool? BeingUsed,
        bool? LicenceToOccupyBeingUsed,
        bool? Received,
        bool? Cleared,
        bool? EmailSigned,
        bool? SaveSigned,
        bool? ReceiveSigned
    ) : IRequest<Result<bool>>;

    internal class UpdateTenancyAtWillTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateTenancyAtWillTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateTenancyAtWillTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            var notApplicable = request.BeingUsed == false && request.LicenceToOccupyBeingUsed == false;

            tasksData.TenancyAtWillBeingUsed = request.BeingUsed;
            tasksData.TenancyAtWillLicenceToOccupyBeingUsed = request.LicenceToOccupyBeingUsed;
            tasksData.TenancyAtWillNotApplicable = notApplicable;

            if (notApplicable)
            {
                tasksData.TenancyAtWillReceived = null;
                tasksData.TenancyAtWillCleared = null;
                tasksData.TenancyAtWillEmailSigned = null;
                tasksData.TenancyAtWillReceiveSigned = null;
                tasksData.TenancyAtWillSaveSigned = null;
            }
            else
            {
                tasksData.TenancyAtWillReceived = request.Received;
                tasksData.TenancyAtWillCleared = request.Cleared;
                tasksData.TenancyAtWillEmailSigned = request.EmailSigned;
                tasksData.TenancyAtWillSaveSigned = request.SaveSigned;
                tasksData.TenancyAtWillReceiveSigned = request.ReceiveSigned;
            }

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}