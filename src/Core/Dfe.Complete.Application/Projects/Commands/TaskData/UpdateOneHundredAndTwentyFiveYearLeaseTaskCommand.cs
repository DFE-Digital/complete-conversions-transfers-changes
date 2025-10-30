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
    public record UpdateOneHundredAndTwentyFiveYearLeaseTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Email,
        bool? Receive,
        bool? Save
    ) : IRequest<Result<bool>>;

    internal class UpdateOneHundredAndTwentyFiveYearLeaseTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateOneHundredAndTwentyFiveYearLeaseTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateOneHundredAndTwentyFiveYearLeaseTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");
             
            tasksData.OneHundredAndTwentyFiveYearLeaseNotApplicable = request.NotApplicable;
            tasksData.OneHundredAndTwentyFiveYearLeaseEmail = request.NotApplicable == true ? null : request.Email;
            tasksData.OneHundredAndTwentyFiveYearLeaseReceive = request.NotApplicable == true ? null : request.Receive;
            tasksData.OneHundredAndTwentyFiveYearLeaseSaveLease = request.NotApplicable == true ? null : request.Save;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
